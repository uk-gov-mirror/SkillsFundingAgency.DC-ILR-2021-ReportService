﻿using Autofac;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class EasDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IlrReferenceDataProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<ValidIlrProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidIlr);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);
            builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);
            builder.RegisterType<EasProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Eas);
        }
    }
}
