﻿using Aspose.Cells;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Mapper;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationSchemaErrorsReport : AbstractReport, IReport
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileProviderService<List<ValidationError>> _ilrValidationErrorsProvider;
        private readonly IValidationSchemaErrorsReportBuilder _validationSchemaErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;

        private FileValidationResult _ilrValidationResult;

        public ValidationSchemaErrorsReport(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IFileProviderService<List<ValidationError>> ilrValidationErrorsProvider,
            IValidationSchemaErrorsReportBuilder validationSchemaErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider) :
            base(valueProvider)
        {
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _ilrValidationErrorsProvider = ilrValidationErrorsProvider;
            _validationSchemaErrorsReportBuilder = validationSchemaErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationSchemaErrorReport;

        public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<string> reportOutputFileNames = new List<string>();
            reportServiceContext.Ukprn = GetUkPrn(reportServiceContext.Filename);
            var externalFileName = GetFilename(reportServiceContext);
            List<ValidationError> ilrValidationErrors = await _ilrValidationErrorsProvider.ProvideAsync(reportServiceContext, cancellationToken);
            var validationErrorModels = _validationSchemaErrorsReportBuilder.Build(ilrValidationErrors);
            var list = await PersistValidationErrorsReport(validationErrorModels, reportServiceContext, externalFileName, cancellationToken);
            reportOutputFileNames.AddRange(list);
            
            return reportOutputFileNames;
        }
      
        private async Task<List<string>> PersistValidationErrorsReport(List<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            List<string> filesGenerated = new List<string>();

            using (Stream stream = await _fileService.OpenWriteStreamAsync($"{externalFileName}.csv", reportServiceContext.Container, cancellationToken))
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(stream, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<ValidationErrorMapper, ValidationErrorModel>(csvWriter, validationErrors);
                        csvWriter.Flush();
                        textWriter.Flush();
                    }
                }
            }
            filesGenerated.Add($"{externalFileName}.csv");

            return filesGenerated;
        }

        private string GetFilename(IReportServiceContext reportServiceContext)
        {
            var ukPrn = 124;
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{ukPrn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }
        private int GetUkPrn(string fileName)
        {
            var ukPrn = 99999999;
            try
            {
                var fileNameParts = fileName.Substring(0, fileName.IndexOf('.')).Split('-');
                ukPrn = Convert.ToInt32(fileNameParts[1]);
            }
            catch (Exception ex)
            {
                _logger.LogError("ValidationErrorsSchemaReport - Could not parse UkPRN from the filename");
            }

            return ukPrn;
        }

    }
}