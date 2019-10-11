﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.CommunityLearning
{
    public class CommunityLearningReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(2);
            dependsOn.Should().Contain(DependentDataCatalog.ValidIlr);
            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";
            var sheetName = "CommunityLearningReport";

            var communityLearningReportModelBuilderMock = new Mock<IModelBuilder<ICommunityLearningReport>>();

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var communityLearningReportModel = Mock.Of<ICommunityLearningReport>();

            communityLearningReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(communityLearningReportModel);

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, sheetName)).Returns(worksheet);

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FileName";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "CL Summary of Learners by Non-Single Budget Category Report", OutputTypes.Excel, true)).Returns(fileName);

            var communityLearningReportRenderServiceMock = new Mock<IRenderService<ICommunityLearningReport>>();
            
            var report = NewReport(fileNameServiceMock.Object, communityLearningReportModelBuilderMock.Object, excelServiceMock.Object, communityLearningReportRenderServiceMock.Object);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);
            
            excelServiceMock.Verify(s => s.SaveWorkbookAsync(workbook, fileName, container, cancellationToken));
            communityLearningReportRenderServiceMock.Verify(s => s.Render(communityLearningReportModel, worksheet));
        }

        //[Fact]
        //public async Task SystemTest()
        //{
        //    var container = "Output";
        //    var ukprn = 123456789;
        //    var reportGeneratedAt = new DateTime(2019, 03, 03);

        //    var refDataRoot = new ReferenceDataRoot()
        //    {
        //        Organisations = new List<Organisation>()
        //        {
        //            new Organisation()
        //            {
        //                UKPRN = ukprn,
        //                Name = "OrganisationName"
        //            }
        //        },
        //        MetaDatas = new MetaData()
        //        {
        //            ReferenceDataVersions = new ReferenceDataVersion()
        //            {
        //                OrganisationsVersion = new OrganisationsVersion { Version = "orgVersion" },
        //                EasUploadDateTime = new EasUploadDateTime { UploadDateTime = new DateTime(2019, 01, 01) },
        //                LarsVersion = new LarsVersion { Version = "larsVersion" },
        //                PostcodesVersion = new PostcodesVersion { Version = "postcodeVersion" },
        //                Employers = new EmployersVersion { Version = "employersVersion" }
        //            }
        //        }
        //    };

        //    var message = new Message()
        //    {
        //        Header = new MessageHeader()
        //        {
        //            CollectionDetails = new MessageHeaderCollectionDetails()
        //            {
        //                FilePreparationDate = new DateTime(2019, 01, 02)
        //            }
        //        }
        //    };

        //    var reportServiceDependentDataMock = new Mock<IReportServiceDependentData>();
        //    reportServiceDependentDataMock.Setup(x => x.Get<ReferenceDataRoot>()).Returns(refDataRoot);
        //    reportServiceDependentDataMock.Setup(x => x.Get<IMessage>()).Returns(message);

        //    var periodisedValuesLookupProvider = new Mock<IPeriodisedValuesLookupProvider>();

        //    periodisedValuesLookupProvider.Setup(p => p.Provide(It.IsAny<IEnumerable<FundingDataSources>>(), reportServiceDependentDataMock.Object)).Returns(new PeriodisedValuesLookup());

        //    var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        //    dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(reportGeneratedAt);
        //    dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(reportGeneratedAt)).Returns(reportGeneratedAt);

        //    var fundingSummaryReportModelBuilder = new FundingSummaryReportModelBuilder(periodisedValuesLookupProvider.Object, dateTimeProviderMock.Object);

        //    var reportServiceContextMock = new Mock<IReportServiceContext>();
        //    reportServiceContextMock.Setup(c => c.Container).Returns(container);
        //    reportServiceContextMock.Setup(c => c.Ukprn).Returns(ukprn);
        //    reportServiceContextMock.Setup(c => c.OriginalFilename).Returns("ILR-11111111-1920-20190507-152000-01");
        //    reportServiceContextMock.Setup(c => c.ServiceReleaseVersion).Returns("ServiceReleaseVersion");

        //    var excelService = new ExcelService(new FileServiceStub());

        //    var fileNameServiceMock = new Mock<IFileNameService>();

        //    var fileName = "FundingSummaryReport.xlsx";
        //    fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

        //    var fundingSummaryReportRenderService = new FundingSummaryReportRenderService();

        //    var report = NewReport(
        //        fileNameServiceMock.Object,
        //        fundingSummaryReportModelBuilder,
        //        excelService,
        //        fundingSummaryReportRenderService);

        //    var cancellationToken = CancellationToken.None;

        //    excelService.ApplyLicense();
            
        //    await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentDataMock.Object, cancellationToken);
        //}

        private CommunityLearningReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<ICommunityLearningReport> communityLearningReportModelBuilder = null,
            IExcelService excelService = null,
            IRenderService<ICommunityLearningReport> communityLearningReportRenderService = null)
        {
            return new CommunityLearningReport(fileNameService, communityLearningReportModelBuilder, excelService, communityLearningReportRenderService);
        }
    }
}
