﻿using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship
{
    public class AppsIndicativeEarningsReport : AbstractCsvReport<AppsIndicativeEarningsReportModel, AppsIndicativeEarningsReportMapper>, IReport
    {
        public AppsIndicativeEarningsReport(
            IFileNameService fileNameService,
            IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>> appsIndicativeEarningsModelBuilder,
            ICsvService csvService)
            : base(fileNameService, appsIndicativeEarningsModelBuilder, csvService, ReportTaskNameConstants.AppsIndicativeEarningsReport, "Apps Indicative Earnings Report")
        {
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm36,
                DependentDataCatalog.Ilr,
                DependentDataCatalog.ReferenceData
            };
    }
}
