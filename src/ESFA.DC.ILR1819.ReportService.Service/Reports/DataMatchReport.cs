﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.DasCommitments;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.ReferenceData;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class DataMatchReport : AbstractReportBuilder, IReport
    {
        private static readonly DataMatchModelComparer DataMatchModelComparer = new DataMatchModelComparer();

        private readonly ILogger _logger;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IDasCommitmentsService _dasCommitmentsService;
        private readonly IPeriodProviderService _periodProviderService;
        private readonly IKeyValuePersistenceService _storage;

        private readonly List<DataMatchModel> dataMatchModels;

        public DataMatchReport(
            ILogger logger,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IFM36ProviderService fm36ProviderService,
            IDasCommitmentsService dasCommitmentsService,
            IPeriodProviderService periodProviderService,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IDateTimeProvider dateTimeProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
            : base(dateTimeProvider)
        {
            _logger = logger;
            _ilrProviderService = ilrProviderService;
            _validLearnersService = validLearnersService;
            _fm36ProviderService = fm36ProviderService;
            _dasCommitmentsService = dasCommitmentsService;
            _periodProviderService = periodProviderService;
            _storage = blob;

            dataMatchModels = new List<DataMatchModel>();
            ReportFileName = "Apprenticeship Data Match Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateDataMatchReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            if (!long.TryParse(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString(), out var ukPrn))
            {
                _logger.LogWarning($"Cannot convert {JobContextMessageKey.UkPrn} with value {jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]} to long, can't generate {nameof(DataMatchReport)}");
                return;
            }

            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<FM36Global> fm36Task = _fm36ProviderService.GetFM36Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm36Task, validLearnersTask).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            List<RawEarning> rawEarnings = new List<RawEarning>();
            List<long> ulns = await GetUlnsForValidLearners(ilrFileTask.Result, fm36Task.Result, rawEarnings, jobContextMessage, cancellationToken);

            List<DasCommitment> commitments = await _dasCommitmentsService.GetCommitments(
                ukPrn,
                ulns,
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            BuildReportData(rawEarnings, commitments, ukPrn);

            var jobId = jobContextMessage.JobId;
            var externalFileName = GetExternalFilename(ukPrn.ToString(), jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn.ToString(), jobId, jobContextMessage.SubmissionDateTimeUtc);

            string csv = WriteResults(dataMatchModels);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private void BuildReportData(List<RawEarning> rawEarnings, List<DasCommitment> commitments, long ukPrn)
        {
            foreach (RawEarning rawEarning in rawEarnings)
            {
                var commitmentsToMatch = commitments.Where(x => x.Uln == rawEarning.Uln).ToList();

                if (!commitmentsToMatch.Any())
                {
                    AddError(DataLockValidationMessages.DLOCK_02, rawEarning, rawEarning.Uln.ToString());
                    continue;
                }

                var commitmentsToMatch2 = commitmentsToMatch.Where(x => x.Ukprn == rawEarning.Ukprn).ToList();

                if (!commitmentsToMatch2.Any())
                {
                    AddError(DataLockValidationMessages.DLOCK_01, rawEarning, rawEarning.Ukprn.ToString(), ukPrn.ToString());
                    continue;
                }

                commitmentsToMatch = commitmentsToMatch2;

                var hasStandardCode = rawEarning.StandardCode > 0 ||
                                      commitmentsToMatch.Any(x => x.StandardCode.HasValue && x.StandardCode > 0);

                if (hasStandardCode)
                {
                    commitmentsToMatch2 = commitmentsToMatch.Where(c => c.StandardCode.HasValue &&
                                                                        c.StandardCode.Value == rawEarning.StandardCode)
                        .ToList();

                    if (!commitmentsToMatch2.Any())
                    {
                        foreach (DasCommitment dasCommitment in commitmentsToMatch)
                        {
                            AddError(
                                DataLockValidationMessages.DLOCK_03,
                                rawEarning,
                                rawEarning.StandardCode.ToString(),
                                dasCommitment.StandardCode.GetValueOrDefault(0).ToString());
                        }
                    }
                    else
                    {
                        commitmentsToMatch = commitmentsToMatch2;
                    }
                }

                var hasFrameworkCode = rawEarning.FrameworkCode > 0 ||
                                       commitmentsToMatch.Any(x => x.FrameworkCode.HasValue && x.FrameworkCode > 0);

                if (hasFrameworkCode)
                {
                    commitmentsToMatch2 = commitmentsToMatch.Where(c => c.FrameworkCode.HasValue &&
                                                                        rawEarning.FrameworkCode != 0 &&
                                                                        c.FrameworkCode.Value == rawEarning.FrameworkCode)
                        .ToList();

                    if (!commitmentsToMatch2.Any())
                    {
                        foreach (DasCommitment dasCommitment in commitmentsToMatch)
                        {
                            AddError(
                                DataLockValidationMessages.DLOCK_04,
                                rawEarning,
                                rawEarning.FrameworkCode?.ToString(),
                                dasCommitment.FrameworkCode.GetValueOrDefault(0).ToString());
                        }
                    }
                    else
                    {
                        commitmentsToMatch = commitmentsToMatch2;
                    }
                }

                var hasPathwayCode = rawEarning.PathwayCode > 0 ||
                                     commitmentsToMatch.Any(x => x.PathwayCode.HasValue && x.PathwayCode > 0);

                if (hasPathwayCode)
                {
                    commitmentsToMatch2 = commitmentsToMatch.Where(c => c.PathwayCode.HasValue &&
                                                                        rawEarning.PathwayCode > 0 &&
                                                                        c.PathwayCode.Value == rawEarning.PathwayCode).ToList();

                    if (!commitmentsToMatch2.Any())
                    {
                        foreach (DasCommitment dasCommitment in commitmentsToMatch)
                        {
                            AddError(
                                DataLockValidationMessages.DLOCK_06,
                                rawEarning,
                                rawEarning.PathwayCode?.ToString(),
                                dasCommitment.PathwayCode.GetValueOrDefault(0).ToString());
                        }
                    }
                }

                var hasProgrammeType = rawEarning.ProgrammeType > 0 ||
                                       commitmentsToMatch.Any(x => x.ProgrammeType.HasValue && x.ProgrammeType > 0);

                if (hasProgrammeType)
                {
                    commitmentsToMatch2 = commitmentsToMatch.Where(c => c.ProgrammeType.HasValue &&
                                                                        rawEarning.ProgrammeType != 0 &&
                                                                        c.ProgrammeType.Value == rawEarning.ProgrammeType)
                        .ToList();

                    if (!commitmentsToMatch2.Any())
                    {
                        foreach (DasCommitment dasCommitment in commitmentsToMatch)
                        {
                            AddError(
                                DataLockValidationMessages.DLOCK_05,
                                rawEarning,
                                rawEarning.ProgrammeType?.ToString(),
                                dasCommitment.ProgrammeType.GetValueOrDefault(0).ToString());
                        }
                    }
                    else
                    {
                        commitmentsToMatch = commitmentsToMatch2;
                    }
                }

                commitmentsToMatch2 = commitmentsToMatch.Where(c => c.AgreedCost == rawEarning.AgreedPrice).ToList();

                if (!commitmentsToMatch2.Any())
                {
                    foreach (DasCommitment dasCommitment in commitmentsToMatch)
                    {
                        AddError(
                            DataLockValidationMessages.DLOCK_07,
                            rawEarning,
                            rawEarning.AgreedPrice?.ToString() ?? string.Empty,
                            dasCommitment.AgreedCost.ToString(CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    commitmentsToMatch = commitmentsToMatch2;
                }

                commitmentsToMatch2 = commitmentsToMatch.Where(c =>
                {
                    if (c.IsVersioned)
                    {
                        return c.EffectiveFromDate <= rawEarning.EpisodeEffectiveTnpStartDate;
                    }

                    return c.StartDate <= rawEarning.EpisodeEffectiveTnpStartDate;
                }).ToList();

                if (!commitmentsToMatch2.Any())
                {
                    AddError(
                        DataLockValidationMessages.DLOCK_09,
                        rawEarning,
                        rawEarning.LearningDeliveryStartDate.ToString("dd/MM/yyyy HH:mm"));
                }
                else
                {
                    commitmentsToMatch = commitmentsToMatch2;
                }

                var censusDate = CalculateOnProgCensusDate(rawEarning);

                var withdrawnCommitments = commitmentsToMatch
                    .Where(x => x.PaymentStatus == (int)PaymentStatus.Withdrawn || x.WithdrawnOnDate.HasValue)
                    .ToList();
                var activeWithdrawnCommitments = withdrawnCommitments
                    .Where(x => x.WithdrawnOnDate >= censusDate)
                    .ToList();

                if (withdrawnCommitments.Any() && !activeWithdrawnCommitments.Any())
                {
                    AddError(DataLockValidationMessages.DLOCK_10, rawEarning);
                }

                if (commitmentsToMatch.Any(x => x.PaymentStatus == (int)PaymentStatus.Paused))
                {
                    AddError(DataLockValidationMessages.DLOCK_12, rawEarning);
                }
            }

            dataMatchModels.Sort(DataMatchModelComparer);
        }

        private void AddError(string ruleName, RawEarning rawEarning, string ilrValue = "", string calculatedValue = "")
        {
            dataMatchModels.Add(new DataMatchModel
            {
                LearnRefNumber = rawEarning.LearnRefNumber,
                Uln = rawEarning.Uln,
                AimSeqNumber = rawEarning.AimSeqNumber,
                RuleName = ruleName,
                Description = DataLockValidationMessages.Validations.FirstOrDefault(x => x.RuleId == ruleName)?.ErrorMessage ?? "Description not found",
                ILRValue = ilrValue,
                ApprenticeshipServiceValue = calculatedValue,
                PriceEpisodeStartDate = rawEarning.EpisodeStartDate?.ToString("dd/MM/yyyy HH:mm") ?? rawEarning.EpisodeEffectiveTnpStartDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PriceEpisodeActualEndDate = rawEarning.EndDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
            });
        }

        private async Task<List<long>> GetUlnsForValidLearners(
            IMessage message,
            FM36Global fm36Data,
            List<RawEarning> rawEarnings,
            IJobContextMessage jobContextMessage,
            CancellationToken cancellationToken)
        {
            int period = await _periodProviderService.GetPeriod(jobContextMessage, cancellationToken);
            List<long> ulns = new List<long>();
            foreach (ILearner learner in message.Learners)
            {
                FM36Learner fm36Entry =
                    fm36Data.Learners.SingleOrDefault(x => string.Equals(x.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase));

                if (fm36Entry == null)
                {
                    continue;
                }

                ulns.Add(learner.ULN);

                foreach (ILearningDelivery learnerLearningDelivery in learner.LearningDeliveries.Where(x => x.FundModel == 36 && x.LearningDeliveryFAMs.Any(y => string.Equals(y.LearnDelFAMType, "ACT", StringComparison.OrdinalIgnoreCase) && string.Equals(y.LearnDelFAMCode, "1", StringComparison.OrdinalIgnoreCase))))
                {
                    List<PriceEpisode> priceEpisode = fm36Entry.PriceEpisodes.Where(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber == learnerLearningDelivery.AimSeqNumber && string.Equals(x.PriceEpisodeValues.PriceEpisodeContractType, "Levy Contract", StringComparison.OrdinalIgnoreCase)).ToList();

                    if (!priceEpisode.Any())
                    {
                        _logger.LogWarning($"Can't find any price episodes for learner {learner.LearnRefNumber} with learning delivery aim sequence no. {learnerLearningDelivery.AimSeqNumber} skipping {nameof(DataMatchReport)} {nameof(RawEarning)} model");
                        continue;
                    }

                    foreach (PriceEpisode episode in priceEpisode)
                    {
                        RawEarning rawEarning = new RawEarning();
                        rawEarning.Uln = learner.ULN;
                        rawEarning.LearnRefNumber = learner.LearnRefNumber;
                        rawEarning.AimSeqNumber = learnerLearningDelivery.AimSeqNumber;
                        rawEarning.Ukprn = message.HeaderEntity.SourceEntity.UKPRN;
                        rawEarning.EpisodeStartDate = episode.PriceEpisodeValues.EpisodeStartDate;
                        rawEarning.EpisodeEffectiveTnpStartDate =
                            episode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate;
                        rawEarning.EndDate = episode.PriceEpisodeValues.PriceEpisodeActualEndDate ??
                                             episode.PriceEpisodeValues.PriceEpisodePlannedEndDate;
                        rawEarning.Period = period;
                        rawEarning.ProgrammeType = learnerLearningDelivery.ProgTypeNullable;
                        rawEarning.FrameworkCode = learnerLearningDelivery.FworkCodeNullable;
                        rawEarning.PathwayCode = learnerLearningDelivery.PwayCodeNullable;
                        rawEarning.StandardCode = learnerLearningDelivery.StdCodeNullable;
                        rawEarning.AgreedPrice = episode.PriceEpisodeValues.PriceEpisodeTotalTNPPrice;

                        rawEarning.LearningDeliveryStartDate = learnerLearningDelivery.LearnStartDate;

                        rawEarnings.Add(rawEarning);
                    }
                }
            }

            return ulns;
        }

        private DateTime CalculateOnProgCensusDate(RawEarning earning)
        {
            int month = _periodProviderService.MonthFromPeriod(earning.Period);
            int year = YearFromPeriod(earning.Period, earning.EpisodeStartDate ?? new DateTime(9999, 01, 01));

            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, lastDayOfMonth);
        }

        private int YearFromPeriod(int period, DateTime episodeStartDate)
        {
            int month = _periodProviderService.MonthFromPeriod(period);
            DateTime startOfAcademicYear = StartOfAcademicYearFromEpisodeStartDate(episodeStartDate);
            if (month < 8)
            {
                return startOfAcademicYear.Year + 1;
            }

            return startOfAcademicYear.Year;
        }

        private DateTime StartOfAcademicYearFromEpisodeStartDate(DateTime episodeStartDate)
        {
            int month = episodeStartDate.Month;
            if (month < 8)
            {
                return new DateTime(episodeStartDate.Year - 1, 8, 1);
            }

            return new DateTime(episodeStartDate.Year, 8, 1);
        }

        private string WriteResults(IReadOnlyCollection<DataMatchModel> models)
        {
            using (var ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<DataMatchReportMapper, DataMatchModel>(csvWriter, models);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
