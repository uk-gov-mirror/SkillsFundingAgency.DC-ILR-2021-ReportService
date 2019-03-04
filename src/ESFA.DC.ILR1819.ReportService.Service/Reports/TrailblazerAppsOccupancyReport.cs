﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
using ESFA.DC.ILR1819.ReportService.Service.Helper;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class TrailblazerAppsOccupancyReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IFM81TrailBlazerProviderService _fm81TrailBlazerProviderService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IValidLearnersService _validLearnersService;

        private readonly ITrailblazerAppsOccupancyModelBuilder _trailblazerAppsOccupancyModelBuilder;

        public TrailblazerAppsOccupancyReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IFM81TrailBlazerProviderService fm81TrailBlazerProviderService,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            ILarsProviderService larsProviderService,
            ITrailblazerAppsOccupancyModelBuilder trailblazerAppsOccupancyModelBuilder,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IValueProvider valueProvider,
            IDateTimeProvider dateTimeProvider)
            : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;

            _fm81TrailBlazerProviderService = fm81TrailBlazerProviderService;
            _validLearnersService = validLearnersService;
            _ilrProviderService = ilrProviderService;
            _larsProviderService = larsProviderService;

            _trailblazerAppsOccupancyModelBuilder = trailblazerAppsOccupancyModelBuilder;

            ReportFileName = "Trailblazer Apprenticeships Occupancy Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateTrailblazerAppsOccupancyReport;
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<FM81Global> fm81Task =
                _fm81TrailBlazerProviderService.GetFM81Data(reportServiceContext, cancellationToken);
            Task<List<string>> validLearnersTask =
                _validLearnersService.GetLearnersAsync(reportServiceContext, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm81Task, validLearnersTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<ILearner> learners =
                ilrFileTask.Result?.Learners?.Where(x => validLearnersTask.Result.Contains(x.LearnRefNumber)).ToList();

            if (learners == null)
            {
                _logger.LogWarning("Failed to get learners for Trailblazer Apprenticeships Occupancy Report");
                return;
            }

            string[] learnAimRefs = learners.SelectMany(x => x.LearningDeliveries).Select(x => x.LearnAimRef).Distinct().ToArray();
            Dictionary<string, LarsLearningDelivery> larsLearningDeliveries = await _larsProviderService.GetLearningDeliveriesAsync(learnAimRefs, cancellationToken);

            var fm81Data = fm81Task.Result;
            var trailblazerAppsOccupancyModels = new List<TrailblazerAppsOccupancyModel>();

            foreach (var learner in learners)
            {
                FM81Learner fm81Learner = fm81Data?.Learners?.SingleOrDefault(x => string.Equals(x.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase));
                foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                {
                    if (!CheckIsApplicableLearner(learningDelivery))
                    {
                        continue;
                    }

                    LarsLearningDelivery larsDelivery = larsLearningDeliveries.SingleOrDefault(x => string.Equals(x.Key, learningDelivery.LearnAimRef, StringComparison.OrdinalIgnoreCase)).Value;
                    LearningDelivery ruleBaseLearningDelivery = fm81Learner?.LearningDeliveries
                        ?.SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber);
                    trailblazerAppsOccupancyModels.Add(_trailblazerAppsOccupancyModelBuilder
                        .BuildTrailblazerAppsOccupancyModel(
                            learner,
                            learningDelivery,
                            larsDelivery,
                            ruleBaseLearningDelivery));
                }
            }

            trailblazerAppsOccupancyModels.Sort(new TrailblazerAppsOccupancyModelComparer());

            var csv = GetReportCsv(trailblazerAppsOccupancyModels);

            var jobId = reportServiceContext.JobId;
            var ukPrn = reportServiceContext.Ukprn.ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);

            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private bool CheckIsApplicableLearner(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == 81 || learningDelivery.FundModel == 25;
        }

        private string GetReportCsv(List<TrailblazerAppsOccupancyModel> listModels)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<TrailblazerAppsOccupancyMapper, TrailblazerAppsOccupancyModel>(csvWriter, listModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}