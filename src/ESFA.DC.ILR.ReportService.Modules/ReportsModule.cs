﻿using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            RegisterValidationReports(containerBuilder);
          
            RegisterDevolvedAdultEducationOccupancyReport(containerBuilder);

            RegisterFundingSummaryReport(containerBuilder);

            containerBuilder.RegisterType<CsvService>().As<ICsvService>();
            containerBuilder.RegisterType<ExcelService>().As<IExcelService>();
        }

        private void RegisterValidationReports(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            containerBuilder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();
            containerBuilder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>();
            containerBuilder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>();
        }

        private void RegisterDevolvedAdultEducationOccupancyReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReport>().As<IReport>();
            containerBuilder.RegisterType<DevolvedAdultEducationOccupancyReportModelBuilder>().As<IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>>();
        }

        protected virtual void RegisterFundingSummaryReport(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingSummaryReport>().As<IReport>();
            containerBuilder.RegisterType<FundingSummaryReportModelBuilder>().As<IModelBuilder<IFundingSummaryReport>>();
            containerBuilder.RegisterType<FundingSummaryReportRenderService>().As<IRenderService<IFundingSummaryReport>>();
            containerBuilder.RegisterType<PeriodisedValuesLookupProvider>().As<IPeriodisedValuesLookupProvider>();
        }
    }
}
