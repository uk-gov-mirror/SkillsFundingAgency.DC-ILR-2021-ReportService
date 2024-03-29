﻿using Autofac;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.ILR.ReportService.Data.Eas;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using Microsoft.EntityFrameworkCore;
using EasProvider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.EasProvider;
using Fm25Provider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.Fm25Provider;
using Fm35Provider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.Fm35Provider;
using Fm36Provider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.Fm36Provider;
using Fm81Provider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.Fm81Provider;
using Fm99Provider = ESFA.DC.ILR.ReportService.Data.Eas.Providers.Fm99Provider;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class EasDataModule : Module
    {
        private IDatabaseConfiguration _databaseConfiguration;

        public EasDataModule(IDatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterDatabases(builder);

            builder.RegisterType<IlrReferenceDataProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<ValidIlrProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidIlr);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);
            builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);
            builder.RegisterType<EasProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Eas);
            builder.RegisterType<FrmReferenceDataProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Frm);
            builder.RegisterType<ReportServiceContextKeysMutator>().As<IReportServiceContextKeysMutator>();
        }

        private void RegisterDatabases(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EasContext>().As<IEasdbContext>();            
            containerBuilder.Register(container => new DbContextOptionsBuilder<EasContext>()
                .UseSqlServer(_databaseConfiguration.EasDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<EasContext>>().SingleInstance();

            containerBuilder.RegisterType<PostcodesContext>().As<IPostcodesContext>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<PostcodesContext>()
                .UseSqlServer(_databaseConfiguration.PostcodesDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<PostcodesContext>>().SingleInstance();

            containerBuilder.RegisterType<ILR2021_DataStoreEntities>().As<IILR2021_DataStoreEntities>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<ILR2021_DataStoreEntities>()
                .UseSqlServer(_databaseConfiguration.IlrDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR2021_DataStoreEntities>>().SingleInstance();

            containerBuilder.RegisterType<ILR2021_DataStoreEntities>().As<IILR2021_DataStoreEntities>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<ILR2021_DataStoreEntities>()
                .UseSqlServer(_databaseConfiguration.IlrDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR2021_DataStoreEntities>>().SingleInstance();
        }
    }
}
