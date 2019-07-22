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
                                    new FundLineGroup(@"ILR Total 16-18 Apprenticeship Frameworks (£)", currentPeriod, FundModels.FM35, new [] { FundLineConstants.Apprenticeship1618 }, periodisedValues)
                                        .WithFundLine("ILR 16-18 Apprenticeship Frameworks Programme Funding (£)", new [] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                                        .WithFundLine("ILR 16-18 Apprenticeship Frameworks Learning Support (£)", new [] { AttributeConstants.Fm35LearnSuppFundCash })
                                }),
                            new FundingSubCategory(
                                @"16-18 Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    new FundLineGroup(@"ILR Total 16-18 Trailblazer Apprenticeships (£)", currentPeriod, FundModels.FM81, new [] { FundLineConstants.TrailblazerApprenticeship1618 }, periodisedValues)
                                        .WithFundLine(@"ILR 16-18 Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English) (£)", new [] { AttributeConstants.Fm81CoreGovContPayment, AttributeConstants.Fm81MathEngBalPayment, AttributeConstants.Fm81MathEngOnProgPayment })
                                        .WithFundLine(@"ILR 16-18 Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16-18) (£)", new []{ AttributeConstants.Fm81AchPayment, AttributeConstants.Fm81SmallBusPayment, AttributeConstants.Fm81YoungAppPayment })
                                        .WithFundLine(@"ILR 16-18 Trailblazer Apprenticeships Learning Support (£)", new [] { AttributeConstants.Fm81LearnSuppFundCash })
                                }),
                            new FundingSubCategory(
                                @"16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery",
                                new List<IFundLineGroup>()
                                {
                                    new FundLineGroup(@"ILR Total 16-18 Non-Levy Contracted Apprenticeships (£)", currentPeriod, FundModels.FM36, new [] { FundLineConstants.NonLevyApprenticeship1618, FundLineConstants.NonLevyApprenticeship1618NonProcured }, periodisedValues)
                                        .WithFundLine(@"ILR 16-18 Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", new [] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment })
                                        .WithFundLine(@"...of which Indicative Government Co-Investment Earnings (£)", new [] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest })
                                        .WithFundLine(@"ILR 16-18 Non-Levy Contracted Apprenticeships Maths & English Programme Funding (£)", new []{ AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                                        .WithFundLine(@"ILR 16-18 Non-Levy Contracted Apprenticeships Framework Uplift (£)", new [] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                                        .WithFundLine(@"ILR 16-18 Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", new [] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                                        .WithFundLine(@"ILR 16-18 Non-Levy Contracted Apprenticeships Additional Payments for Providers(£)", new []{ AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                                        .WithFundLine(@"ILR 16 - 18 Non - Levy Contracted Apprenticeships Additional Payments for Employers(£)", new [] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                                        .WithFundLine(@"ILR 16 - 18 Non - Levy Contracted Apprenticeships Learning Support(£)", new [] { AttributeConstants.Fm36LearnSuppFundCash })
                                })
                        })
                });
        }
    }
}
