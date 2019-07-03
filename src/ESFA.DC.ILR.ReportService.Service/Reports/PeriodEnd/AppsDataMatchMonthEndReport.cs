﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public class AppsDataMatchMonthEndReport : AbstractReport, IReport
    {
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM36PeriodEndProviderService _fm36ProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;

        private readonly IAppsDataMatchMonthEndModelBuilder _modelBuilder;

        public AppsDataMatchMonthEndReport(
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IFM36PeriodEndProviderService fm36ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IAppsDataMatchMonthEndModelBuilder modelBuilder,
            ILogger logger)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrProviderService = ilrProviderService;
            _fm36ProviderService = fm36ProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _modelBuilder = modelBuilder;
        }

        public override string ReportFileName => "Apprenticeship Data Match Report";

        public override string ReportTaskName => ReportTaskNameConstants.AppsDataMatchMonthEndReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            string csv = await GetCsv(reportServiceContext, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var appsDataMatchMonthEndModels = new List<AppsDataMatchMonthEndModel>();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsDataMatchMonthEndMapper, AppsDataMatchMonthEndModel>(csvWriter, appsDataMatchMonthEndModels);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}