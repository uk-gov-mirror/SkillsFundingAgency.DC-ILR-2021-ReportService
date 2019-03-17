﻿using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders.PeriodEnd
{
    public interface IFundingSummaryPeriodEndModelBuilder
    {
        FundingSummaryPeriodEndModel BuildModel(ILearner learner);
    }
}