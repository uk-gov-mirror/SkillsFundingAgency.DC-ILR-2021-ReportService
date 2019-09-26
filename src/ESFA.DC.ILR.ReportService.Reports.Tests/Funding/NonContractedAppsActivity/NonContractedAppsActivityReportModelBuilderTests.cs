﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.FCS;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportModelBuilderTests
    {
        [Fact]
        public void BuildLearnerDictionary()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner { LearnRefNumber = "Learner1" },
                    new TestLearner { LearnRefNumber = "Learner2" },
                    new TestLearner { LearnRefNumber = "Learner3" },
                    new TestLearner { LearnRefNumber = "Learner4" },
                    new TestLearner { LearnRefNumber = "Learner5" },
                    new TestLearner { LearnRefNumber = "Learner6" },
                }
            };

            var expectedDictionary = new Dictionary<string, ILearner>
            {
                { "Learner1", new TestLearner { LearnRefNumber = "Learner1" }},
                { "Learner2", new TestLearner { LearnRefNumber = "Learner2" }},
                { "Learner3", new TestLearner { LearnRefNumber = "Learner3" }},
                { "Learner4", new TestLearner { LearnRefNumber = "Learner4" }},
                { "Learner5", new TestLearner { LearnRefNumber = "Learner5" }},
                { "Learner6", new TestLearner { LearnRefNumber = "Learner6" }},
            };

            NewReport().BuildLearnerDictionary(message).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFm36LearningDeliveryDictionary()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner { LearnRefNumber = "Learner1", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } } },
                    new TestLearner { LearnRefNumber = "Learner2", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 70 } } },
                    new TestLearner { LearnRefNumber = "Learner3", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 }, new TestLearningDelivery { AimSeqNumber = 2, FundModel = 36 } } },
                }
            };

            var expectedDictionary = new Dictionary<string, Dictionary<int, ILearningDelivery>>
            {
                 { "Learner1", new Dictionary<int, ILearningDelivery> {{ 1, new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } }} },
                 { "Learner2", new Dictionary<int, ILearningDelivery>() },
                 { "Learner3", new Dictionary<int, ILearningDelivery> {{ 1, new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } }, { 2, new TestLearningDelivery { AimSeqNumber = 2, FundModel = 36 } } } },
            };

            NewReport().BuildFm36LearningDeliveryDictionary(message).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFcsContractMapping()
        {
            var contractAllocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation { ContractAllocationNumber = "ConRef1", FundingStreamPeriodCode = "FSPC1" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef2", FundingStreamPeriodCode = "FSPC1" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef3", FundingStreamPeriodCode = "FSPC2" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef4", FundingStreamPeriodCode = "FSPC3" },
            };
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner {
                        LearnRefNumber = "Learner1",
                        LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36, ConRefNumber = "ConRef1" } } },
                    new TestLearner {
                        LearnRefNumber = "Learner2",
                        LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 70, ConRefNumber = "ConRef2" } } },
                    new TestLearner {
                        LearnRefNumber = "Learner3",
                        LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36, ConRefNumber = "ConRef1" }, new TestLearningDelivery { AimSeqNumber = 2, FundModel = 36 } } },
                }
            };

            var expectedDictionary = new Dictionary<string, Dictionary<int, string>>
            {
                 { "Learner1", new Dictionary<int, string> {{ 1, "FSPC1" }} },
                 { "Learner2", new Dictionary<int, string>() },
                 { "Learner3", new Dictionary<int, string> {{ 1, "FSPC1" }, { 2, null }} },
            };

           NewReport().BuildFcsContractMapping(contractAllocations, message).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void ValidContractMappings()
        {
            var mappings = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
            };

            NewReport().ValidContractMappings.Should().BeEquivalentTo(mappings);
        }

        [Fact]
        public void BuildLarsLearningDeliveryDictionary()
        {
            var larsLearningDeliveries = new List<LARSLearningDelivery>
            {
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef1",
                    LearnAimRefTitle = "Title1"
                },
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef2",
                    LearnAimRefTitle = "Title2"
                }
            };

            var reportBuilder = NewReport().BuildLARSDictionary(larsLearningDeliveries);

            reportBuilder.Should().HaveCount(2);
            reportBuilder.Should().ContainKeys(new string[] { "LearnAimRef1", "LearnAimRef2" });
        }

        private NonContractedAppsActivityReportModelBuilder NewReport(IIlrModelMapper ilrModelMapper = null)
        {
            return new NonContractedAppsActivityReportModelBuilder(ilrModelMapper);
        }
    }
}
