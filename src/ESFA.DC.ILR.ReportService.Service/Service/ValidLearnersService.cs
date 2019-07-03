﻿using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public sealed class ValidLearnersService : BaseLearnersService, IValidLearnersService
    {
        public ValidLearnersService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            IReportServiceConfiguration reportServiceConfiguration)
        : base("ValidLearnRefNumbers", logger, storage, jsonSerializationService, reportServiceConfiguration)
        {
        }
    }
}