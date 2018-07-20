﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.AttributeFilters;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.Auditing;
using ESFA.DC.Auditing.Dto;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.CollectionsManagement.Services;
using ESFA.DC.CollectionsManagement.Services.Interface;
using ESFA.DC.DateTime.Provider;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Service;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ILR1819.ReportService.Stateless.Handlers;
using ESFA.DC.ILR1819.ReportService.Stateless.Mappers;
using ESFA.DC.ILR1819.ReportService.Stateless.Modules;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.IO.Redis;
using ESFA.DC.IO.Redis.Config.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Stateless
{
    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        public static void Main()
        {
            try
            {
                var builder = BuildContainer(new ConfigurationHelper());

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the stateless service.
                builder.RegisterStatelessService<Stateless>("ESFA.DC.ILR1819.ReportService.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e + Environment.NewLine + (e.InnerException?.ToString() ?? "No inner exception"));
                throw;
            }
        }

        public static ContainerBuilder BuildContainer(IConfigurationHelper configHelper)
        {
            var containerBuilder = new ContainerBuilder();

            var topicAndTaskOptions = configHelper.GetSectionValues<TopicAndTaskSectionOptions>("TopicAndTaskSection");
            containerBuilder.RegisterInstance(topicAndTaskOptions).As<ITopicAndTaskSectionOptions>().SingleInstance();

            var larsConfiguration = configHelper.GetSectionValues<LarsConfiguration>("LarsSection");
            containerBuilder.RegisterInstance(larsConfiguration).As<ILarsConfiguration>().SingleInstance();

            var orgConfiguration = configHelper.GetSectionValues<OrgConfiguration>("OrgSection");
            containerBuilder.RegisterInstance(orgConfiguration).As<IOrgConfiguration>().SingleInstance();

            var collectionsManagementConfiguration =
                configHelper.GetSectionValues<CollectionsManagementConfiguration>("CollectionsManagementSection");
            containerBuilder.RegisterInstance(collectionsManagementConfiguration)
                .As<ICollectionsManagementConfiguration>().SingleInstance();

            // register Cosmos config
            var azureRedisOptions = configHelper.GetSectionValues<RedisOptions>("RedisSection");
            containerBuilder.Register(c => new RedisKeyValuePersistenceConfig(
                    azureRedisOptions.RedisConnectionString))
                .As<IRedisKeyValuePersistenceServiceConfig>().SingleInstance();

            containerBuilder.RegisterType<RedisKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.Redis)
                .InstancePerLifetimeScope();

            // register azure blob storage service
            var azureBlobStorageOptions = configHelper.GetSectionValues<AzureStorageOptions>("AzureStorageSection");
            containerBuilder.Register(c =>
                    new AzureStorageKeyValuePersistenceConfig(
                        azureBlobStorageOptions.AzureBlobConnectionString,
                        azureBlobStorageOptions.AzureBlobContainerName))
                .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();

            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.Blob)
                .InstancePerLifetimeScope();

            // register serialization
            containerBuilder.RegisterType<JsonSerializationService>()
                .As<IJsonSerializationService>();
            containerBuilder.RegisterType<XmlSerializationService>()
                .As<IXmlSerializationService>();

            // get ServiceBus, Azurestorage config values and register container
            var serviceBusOptions =
                configHelper.GetSectionValues<ServiceBusOptions>("ServiceBusSettings");
            containerBuilder.RegisterInstance(serviceBusOptions).As<ServiceBusOptions>().SingleInstance();

            // Version info
            var versionInfo = configHelper.GetSectionValues<VersionInfo>("VersionSection");
            containerBuilder.RegisterInstance(versionInfo).As<IVersionInfo>().SingleInstance();

            // register logger
            var loggerOptions =
                configHelper.GetSectionValues<LoggerOptions>("LoggerSection");
            containerBuilder.RegisterInstance(loggerOptions).As<LoggerOptions>().SingleInstance();
            containerBuilder.RegisterModule<LoggerModule>();

            // auditing
            var auditPublishConfig = new ServiceBusQueueConfig(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.AuditQueueName,
                Environment.ProcessorCount);
            containerBuilder.Register(c => new QueuePublishService<AuditingDto>(
                    auditPublishConfig,
                    c.Resolve<IJsonSerializationService>()))
                .As<IQueuePublishService<AuditingDto>>();
            containerBuilder.RegisterType<Auditor>().As<IAuditor>();

            // get job status queue config values and register container
            var jobStatusQueueOptions =
                configHelper.GetSectionValues<JobStatusQueueOptions>("JobStatusSection");
            containerBuilder.RegisterInstance(jobStatusQueueOptions).As<JobStatusQueueOptions>().SingleInstance();

            // Job Status Update Service
            var jobStatusPublishConfig = new JobStatusQueueConfig(
                jobStatusQueueOptions.JobStatusConnectionString,
                jobStatusQueueOptions.JobStatusQueueName,
                Environment.ProcessorCount);

            containerBuilder.Register(c => new QueuePublishService<JobStatusDto>(
                    jobStatusPublishConfig,
                    c.Resolve<IJsonSerializationService>()))
                .As<IQueuePublishService<JobStatusDto>>();
            containerBuilder.RegisterType<JobStatus.JobStatus>().As<IJobStatus>();

            // register Jobcontext services
            var topicConfig = new ServiceBusTopicConfig(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.TopicName,
                serviceBusOptions.ReportingSubscriptionName,
                Environment.ProcessorCount);
            containerBuilder.Register(c =>
            {
                var topicSubscriptionSevice =
                    new TopicSubscriptionSevice<JobContextDto>(
                        topicConfig,
                        c.Resolve<IJsonSerializationService>(),
                        c.Resolve<ILogger>());
                return topicSubscriptionSevice;
            }).As<ITopicSubscriptionService<JobContextDto>>();

            containerBuilder.Register(c =>
            {
                var topicPublishSevice =
                    new TopicPublishService<JobContextDto>(
                        topicConfig,
                        c.Resolve<IJsonSerializationService>());
                return topicPublishSevice;
            }).As<ITopicPublishService<JobContextDto>>();

            // register message mapper
            containerBuilder.RegisterType<JobContextMessageMapper>()
                .As<IMapper<JobContextMessage, JobContextMessage>>();

            // register MessageHandler
            containerBuilder.RegisterType<MessageHandler>().As<IMessageHandler>().InstancePerLifetimeScope();

            // register the  callback handle when a new message is received from ServiceBus
            containerBuilder.Register<Func<JobContextMessage, CancellationToken, Task<bool>>>(c =>
                c.Resolve<IMessageHandler>().Handle).InstancePerLifetimeScope();

            containerBuilder.RegisterType<EntryPoint>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<JobContextManagerForTopics<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<JobContextMessage>().As<IJobContextMessage>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidationErrorsReport>().As<IValidationErrorsReport>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IlrProviderService>().As<IIlrProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IAllbOccupancyReport>().As<IAllbOccupancyReport>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<LarsProviderService>().As<ILarsProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AllbProviderService>().As<IAllbProviderService>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidLearnersService>().As<IValidLearnersService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<StringUtilitiesService>().As<IStringUtilitiesService>()
                .InstancePerLifetimeScope();

            containerBuilder.Register(context =>
                {
                    var settings = context.Resolve<ICollectionsManagementConfiguration>();
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseSqlServer(
                        settings.CollectionsManagementConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

            return containerBuilder;
        }
    }
}
