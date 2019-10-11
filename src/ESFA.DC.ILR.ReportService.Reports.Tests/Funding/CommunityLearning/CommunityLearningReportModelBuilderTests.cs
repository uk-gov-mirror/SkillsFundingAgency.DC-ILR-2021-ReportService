﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.CommunityLearning
{
    public class CommunityLearningReportModelBuilderTests
    {
        [Fact]
        public void BuildHeaderData()
        {
            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(r => r.OriginalFilename).Returns("Filename");
            reportServiceContextMock.Setup(r => r.Ukprn).Returns(1);
            reportServiceContextMock.Setup(r => r.CollectionYear).Returns("1920");

            var referenceDataRoot = new ReferenceDataRoot
            {
                Organisations = new List<Organisation>
                {
                    new Organisation
                    {
                        UKPRN = 1,
                        Name = "OrgName"
                    }
                }
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                {SummaryPageConstants.ProviderName, "OrgName"},
                {SummaryPageConstants.UKPRN, "1"},
                {SummaryPageConstants.ILRFile, "Filename"},
                {SummaryPageConstants.Year, "1920"},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };

            NewBuilder().BuildHeaderData(reportServiceContextMock.Object, referenceDataRoot).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFooterData()
        {
            var date = new DateTime(2019, 8, 1);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            dateTimeProviderMock.Setup(d => d.GetNowUtc()).Returns(date);
            dateTimeProviderMock.Setup(d => d.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(date);

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(r => r.ServiceReleaseVersion).Returns("ReleaseVersion");

            IMessage message = new TestMessage
            {
                HeaderEntity = new TestHeader
                {
                    CollectionDetailsEntity = new TestCollectionDetails
                    {
                        FilePreparationDate = new DateTime(2019, 8, 1)
                    }
                }
            };

            var referenceDataRoot = new ReferenceDataRoot
            {
                MetaDatas = new MetaData
                {
                    ReferenceDataVersions = new ReferenceDataVersion
                    {
                        OrganisationsVersion = new OrganisationsVersion { Version = "1" },
                        PostcodesVersion = new PostcodesVersion { Version = "1" },
                        Employers = new EmployersVersion { Version = "1" },
                        LarsVersion = new LarsVersion { Version = "1" }
                    }
                }
            };

            var expectedDictionary = new Dictionary<string, string>
            {

                { SummaryPageConstants.ApplicationVersion, "ReleaseVersion" },
                { SummaryPageConstants.FilePreparationDate, "01/08/2019 00:00:00" },
                { SummaryPageConstants.LARSVersion, "1" },
                { SummaryPageConstants.PostcodeVersion, "1" },
                { SummaryPageConstants.OrganisationVersion, "1" },
                { SummaryPageConstants.LargeEmployersVersion, "1" },
                { SummaryPageConstants.ReportGeneratedAt, "00:00:00 on 01/08/2019" },
                { SummaryPageConstants.Page, SummaryPageConstants.DefaultPageNumber }
            };

            NewBuilder(dateTimeProviderMock.Object).BuildFooterData(reportServiceContextMock.Object, message, referenceDataRoot).Should().BeEquivalentTo(expectedDictionary);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        public void HasAnyASLFamTypeForFamCode_True(string famCode)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ASL",
                    LearnDelFAMCode = famCode
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            NewBuilder().HasAnyASLFamTypeForFamCode(learningDeliveryFams, famCode).Should().BeTrue();
        }

        [Theory]
        [InlineData("LDM","5")]
        [InlineData("LDM","4")]
        [InlineData("ASL","5")]
        public void HasAnyASLFamTypeForFamCode_False(string famType, string famCode)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "4"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            NewBuilder().HasAnyASLFamTypeForFamCode(learningDeliveryFams, famCode).Should().BeFalse();
        }

        [Theory]
        [InlineData(2019, 8, 1)]
        [InlineData(2019, 9, 1)]
        [InlineData(2020, 2, 1)]
        [InlineData(2020, 7, 31)]
        public void LearnStartDateIsWithinYear_True(int year, int month, int day)
        {
            NewBuilder().LearnStartDateIsWithinYear(new DateTime(year, month, day)).Should().BeTrue();
        }

        [Theory]
        [InlineData(2019, 7, 1)]
        [InlineData(2019, 7, 31)]
        [InlineData(2020, 8, 1)]
        [InlineData(2020, 12, 31)]
        public void LearnStartDateIsWithinYear_False(int year, int month, int day)
        {
            NewBuilder().LearnStartDateIsWithinYear(new DateTime(year, month, day)).Should().BeFalse();
        }

        [Fact]
        public void IsAdult_True()
        {
            NewBuilder().IsAdult(new DateTime(2000, 8, 1), new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsAdult_True_NullDoB()
        {
            NewBuilder().IsAdult(null, new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsAdult_False()
        {
            NewBuilder().IsAdult(new DateTime(2002, 8, 1), new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void IsSixteenToEighteen_True()
        {
            NewBuilder().IsSixteenToEighteen(new DateTime(2002, 8, 1), new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsSixteenToEighteen_False_NullDoB()
        {
            NewBuilder().IsSixteenToEighteen(null, new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void IsSixteenToEighteen_False()
        {
            NewBuilder().IsSixteenToEighteen(new DateTime(2000, 8, 1), new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void BuildCategoryData()
        {
            var message = TestMessage();

            var expectedData = new List<CommunityLearningData>
            {
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 8, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = true,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 2,
                    LearnStartDate = new DateTime(2019, 10, 1),
                    EarliestStartDate = false,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = true,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner2",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 8, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner3",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 6, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = false,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
            };

            NewBuilder().BuildCategoryData(message).Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void BuildCategories()
        {
            var categoryData = new List<CommunityLearningData>
            {
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 8, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = true,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 2,
                    LearnStartDate = new DateTime(2019, 10, 1),
                    EarliestStartDate = false,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = true,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner2",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 8, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner3",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2019, 6, 1),
                    EarliestStartDate = true,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = false,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
            };

            var result = NewBuilder().BuildCategories(categoryData).ToArray();

            result[0].CategoryName.Should().Be(ReportCategoryConstants.TotalCommunityLearning);
            result[1].CategoryName.Should().Be(ReportCategoryConstants.PersonalAndCommunity);
            result[2].CategoryName.Should().Be(ReportCategoryConstants.NeighbourhoodLearning);
            result[3].CategoryName.Should().Be(ReportCategoryConstants.FamilyEnglishMathsAndLanguage);
            result[4].CategoryName.Should().Be(ReportCategoryConstants.WiderFamilyLearning);

            result[0].TotalLearners.Should().Be(4);
            result[0].TotalStartedInFundingYear.Should().Be(2);
            result[0].TotalEnrolmentsInFundingYear.Should().Be(3);

            result[1].TotalLearners.Should().Be(2);
            result[1].TotalStartedInFundingYear.Should().Be(1);
            result[1].TotalEnrolmentsInFundingYear.Should().Be(1);

            result[2].TotalLearners.Should().Be(1);
            result[2].TotalStartedInFundingYear.Should().Be(0);
            result[2].TotalEnrolmentsInFundingYear.Should().Be(1);

            result[3].TotalLearners.Should().Be(0);
            result[3].TotalStartedInFundingYear.Should().Be(0);
            result[3].TotalEnrolmentsInFundingYear.Should().Be(0);

            result[4].TotalLearners.Should().Be(1);
            result[4].TotalStartedInFundingYear.Should().Be(1);
            result[4].TotalEnrolmentsInFundingYear.Should().Be(1);
        }    

        private IMessage TestMessage() => new TestMessage
        {
            Learners = new TestLearner[]
            {
                new TestLearner
                {
                    LearnRefNumber = "Learner1",
                    DateOfBirthNullable = new DateTime(2000, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2019, 8, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "4"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 2,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2019, 10, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "2"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        }
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner2",
                    DateOfBirthNullable = new DateTime(2000, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2019, 8, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner3",
                    DateOfBirthNullable = new DateTime(1990, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2019, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner4",
                    DateOfBirthNullable = new DateTime(1990, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2019, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "LDM",
                                    LearnDelFAMCode = "034"
                                }
                            }
                        },
                    }
                },
                 new TestLearner
                {
                    LearnRefNumber = "Learner5",
                    DateOfBirthNullable = new DateTime(1990, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 70,
                            LearnStartDate = new DateTime(2019, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
            }
        };

        private CommunityLearningReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new CommunityLearningReportModelBuilder(dateTimeProvider);
        }
    }
}
