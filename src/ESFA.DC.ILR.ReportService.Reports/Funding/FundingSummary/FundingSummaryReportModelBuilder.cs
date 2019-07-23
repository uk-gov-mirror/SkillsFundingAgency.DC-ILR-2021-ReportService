﻿using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilder : IModelBuilder<FundingSummaryReportModel>
    {
        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;

        private readonly IEnumerable<FundModels> _fundModels = new[]
        {
            FundModels.FM35,
        };

        public FundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider )
        {
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
        }

        public FundingSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValues = _periodisedValuesLookupProvider.Provide(_fundModels, reportServiceDependentData);

            var currentPeriod = reportServiceContext.ReturnPeriod;

            return new FundingSummaryReportModel(
                new List<IFundingCategory>()
                {
                    new FundingCategory(
                        @"Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory(
                                @"16-18 Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35FundLineGroup("16-18", "Apprenticeship Frameworks", currentPeriod, new [] { FundLineConstants.Apprenticeship1618 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"16-18 Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("16-18", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1618 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery",
                                new List<IFundLineGroup>()
                                {
                                    BuildNonLevyApprenticeshipsFundLineGroup("16-18", currentPeriod, new [] { FundLineConstants.NonLevyApprenticeship1618, FundLineConstants.NonLevyApprenticeship1618NonProcured }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"19-23 Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35FundLineGroup("19-23", "Apprenticeship Frameworks", currentPeriod, new [] { FundLineConstants.Apprenticeship1923 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"19-23 Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("19-23", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1923 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"24+ Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35FundLineGroup("24+", "Apprenticeship Frameworks", currentPeriod, new [] { FundLineConstants.Apprenticeship24Plus }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"24+ Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("24+", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship24Plus }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"Adult Non-Levy Contracted Apprenticeships - Non-procured delivery",
                                new List<IFundLineGroup>()
                                {
                                    BuildNonLevyApprenticeshipsFundLineGroup("Adult", currentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19Plus, FundLineConstants.NonLevyApprenticeship19PlusNonProcured }, periodisedValues)
                                })
                        }),
                    new FundingCategory(
                        "Apprenticeships – Employers on Apprenticeship Service",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Apprenticeship (Employer on App Service)",
                                new List<IFundLineGroup>()
                                {
                                    BuildApprenticeshipsFundLineGroup("16-18", "Apprenticeship (Employer on App Service)", currentPeriod, new []{ FundLineConstants.ApprenticeshipEmployerOnAppService1618 }, periodisedValues)
                                }),
                            new FundingSubCategory("Adult Apprenticeship (Employer on App Service)",
                                new List<IFundLineGroup>()
                                {
                                    BuildApprenticeshipsFundLineGroup("Adult", "Apprenticeship (Employer on App Service)", currentPeriod, new [] { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus }, periodisedValues)
                                })
                        }),
                    new FundingCategory(
                        "Non-Levy Contracted Apprenticeships Budget - Procured delivery",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Non-Levy Contracted Apprenticeships",
                                new List<IFundLineGroup>()
                                {
                                    BuildApprenticeshipsFundLineGroup("16-18", "Non-Levy Contracted Apprenticeships", currentPeriod, new []{ FundLineConstants.NonLevyApprenticeship1618Procured }, periodisedValues)
                                }),
                            new FundingSubCategory("Adult Non-Levy Contracted Apprenticeships",
                                new List<IFundLineGroup>()
                                {
                                    BuildApprenticeshipsFundLineGroup("Adult", "Non-Levy Contracted Apprenticeships", currentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19PlusProcured }, periodisedValues)
                                })
                        }),
                    new FundingCategory(
                        "16-18 Traineeships Budget",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Traineeships",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm25FundLineGroup(currentPeriod, periodisedValues)
                                })
                        }),
                    new FundingCategory(
                        "19-24 Traineeships - Non-procured delivery",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("19-24 Traineeships",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35FundLineGroup("19-24", "Traineeships", currentPeriod, new [] { FundLineConstants.Traineeship1924, FundLineConstants.Traineeship1924NonProcured }, periodisedValues)
                                })
                        })
                });
        }

        private IFundLineGroup BuildIlrFm25FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup("ILR Total 16-18 Traineeships (£)", currentPeriod, FundModels.FM25, null, periodisedValues)
                .WithFundLine("ILR 16-18 Traineeships Programme Funding (£)", new []{ FundLineConstants.TraineeshipsAdultFunded1618 }, new [] { AttributeConstants.Fm25LrnOnProgPay })
                .WithFundLine("ILR 19-24 Traineeships (16-19 Model) Programme Funding (£)", new [] { FundLineConstants.TraineeshipsAdultFunded19Plus }, new [] { AttributeConstants.Fm25LrnOnProgPay });
        }

        private IFundLineGroup BuildIlrFm35FundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, FundModels.FM35, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                        .WithFundLine($"ILR {ageRange} {description} Frameworks Learning Support (£)", new[] {AttributeConstants.Fm35LearnSuppFundCash});
        }

        private IFundLineGroup BuildTrailblazerApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} Trailblazer Apprenticeships (£)", currentPeriod, FundModels.FM81, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English) (£)", new [] { AttributeConstants.Fm81CoreGovContPayment, AttributeConstants.Fm81MathEngBalPayment, AttributeConstants.Fm81MathEngOnProgPayment })
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16-18) (£)", new []{ AttributeConstants.Fm81AchPayment, AttributeConstants.Fm81SmallBusPayment, AttributeConstants.Fm81YoungAppPayment })
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Learning Support (£)", new [] { AttributeConstants.Fm81LearnSuppFundCash });
        }
             
        private IFundLineGroup BuildNonLevyApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} Non-Levy Contracted Apprenticeships (£)", currentPeriod, FundModels.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment })
                        .WithFundLine($"...of which Indicative Government Co-Investment Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest})
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Maths & English Programme Funding (£)", new[]{ AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Learning Support (£)", new[] {AttributeConstants.Fm36LearnSuppFundCash });
        }

        private IFundLineGroup BuildApprenticeshipsFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, FundModels.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment })
                        .WithFundLine($"...of which Indicative Government Co-Investment Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest })
                        .WithFundLine($"ILR {ageRange} {description} Maths & English Programme Funding (£)", new[] { AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} {description} Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} {description} Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Providers (£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Employers (£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Apprentices (£)", new[] { AttributeConstants.Fm36LearnDelLearnAddPayment })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support(£)", new[] { AttributeConstants.Fm36LearnSuppFundCash });
        }
    }
}
