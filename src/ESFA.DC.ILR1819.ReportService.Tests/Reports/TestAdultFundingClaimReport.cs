﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ILR1819.ReportService.Tests.AutoFac;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public sealed class TestAdultFundingClaimReport
    {
        [Fact]
        public async Task TestAdultFundingClaimReportGeneration()
        {
            string csv = string.Empty;
            byte[] xlsx = null;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10007924_1_Adult Funding Claim Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            Mock<IPostcodeProviderService> postcodeProverServiceMock = new Mock<IPostcodeProviderService>();
            Mock<ILargeEmployerProviderService> largeEmployerProviderService = new Mock<ILargeEmployerProviderService>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            IIntUtilitiesService intUtilitiesService = new IntUtilitiesService();

            EasConfiguration easConfiguration = new EasConfiguration() { EasConnectionString = new TestConfigurationHelper().GetSectionValues<EasConfiguration>("EasSection").EasConnectionString };
            DataStoreConfiguration dataStoreConfiguration = new DataStoreConfiguration()
            {
                ILRDataStoreConnectionString = new TestConfigurationHelper().GetSectionValues<DataStoreConfiguration>("DataStoreSection").ILRDataStoreConnectionString,
                ILRDataStoreValidConnectionString = new TestConfigurationHelper().GetSectionValues<DataStoreConfiguration>("DataStoreSection").ILRDataStoreValidConnectionString
            };
            IEasProviderService easProviderService = new EasProviderService(logger.Object, easConfiguration);
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, intUtilitiesService, dataStoreConfiguration);
            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, intUtilitiesService, dataStoreConfiguration);
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, intUtilitiesService, dataStoreConfiguration);
            IVersionInfo versionInfo = new VersionInfo { ServiceReleaseVersion = "1.2.3.4.5" };

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.xlsx", It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>(
                    (key, value, ct) =>
                    {
                        value.Seek(0, SeekOrigin.Begin);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            value.CopyTo(ms);
                            xlsx = ms.ToArray();
                        }
                    })
                .Returns(Task.CompletedTask);
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            largeEmployerProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");
            postcodeProverServiceMock.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");
            orgProviderService.Setup(x => x.GetProviderName(It.IsAny<IReportServiceContext>(), It.IsAny<CancellationToken>())).ReturnsAsync("Test Provider");

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IValueProvider valueProvider = new ValueProvider();

            AdultFundingClaimReport adultFundingClaimReport = new AdultFundingClaimReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                orgProviderService.Object,
                allbProviderService,
                fm35ProviderService,
                dateTimeProviderMock.Object,
                intUtilitiesService,
                valueProvider,
                larsProviderService.Object,
                easProviderService,
                postcodeProverServiceMock.Object,
                largeEmployerProviderService.Object,
                versionInfo,
                topicsAndTasks,
                new AdultFundingClaimBuilder());

            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(10007924);
            reportServiceContextMock.SetupGet(x => x.Filename).Returns("ILR-10007924-1819-20180704-120055-03");

            MemoryStream memoryStream = new MemoryStream();

            using (memoryStream)
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
                {
                    await adultFundingClaimReport.GenerateReport(reportServiceContextMock.Object, archive, false, CancellationToken.None);
                    //await adultFundingClaimReport.GenerateReport(jobContextMessage, archive, false, CancellationToken.None);
                }

                using (var fileStream = new FileStream($"{filename}.zip", FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }

            xlsx.Should().NotBeNullOrEmpty();

            File.WriteAllBytes($"{filename}.xlsx", xlsx);
        }
    }
}