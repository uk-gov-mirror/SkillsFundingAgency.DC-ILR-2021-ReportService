﻿using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer
{
    public class TrailblazerOccupancyReportClassMap : ClassMap<TrailblazerOccupancyReportModel>
    {
        public TrailblazerOccupancyReportClassMap()
        {
            var index = 0;

            Map(m => m.Learner.LearnRefNumber).Name(@"Learner reference number").Index(++index);
            Map(m => m.Learner.ULN).Name(@"Unique learner number").Index(++index);
            Map(m => m.Learner.DateOfBirthNullable).Name(@"Date of birth").Index(++index);
            Map(m => m.Learner.PMUKPRNNullable).Name(@"Pre-merger UKPRN").Index(++index);
            Map(m => m.Learner.CampId).Name(@"Campus identifier").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.A).Name(@"Provider specified learner monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.B).Name(@"Provider specified learner monitoring (B)").Index(++index);
            Map(m => m.LearningDelivery.AimSeqNumber).Name(@"Aim sequence number").Index(++index);
            Map(m => m.LearningDelivery.LearnAimRef).Name(@"Learning aim reference").Index(++index);
            Map(m => m.LarsLearningDelivery.LearnAimRefTitle).Name(@"Learning aim title").Index(++index);
            Map(m => m.LearningDelivery.SWSupAimId).Name(@"Software supplier aim identifier").Index(++index);
            Map(m => m.LarsLearningDelivery.NotionalNVQLevel).Name(@"Notional NVQ level").Index(++index);
            Map(m => m.LearningDelivery.AimType).Name(@"Aim type").Index(++index);
            Map(m => m.LearningDelivery.StdCodeNullable).Name(@"Apprenticeship standard code").Index(++index);
            Map(m => m.LearningDelivery.FundModel).Name(@"Funding model").Index(++index);
            Map(m => m.LearningDelivery.PriorLearnFundAdjNullable).Name(@"Funding adjustment for prior learning").Index(++index);
            Map(m => m.LearningDelivery.OtherFundAdjNullable).Name(@"Other funding adjustment").Index(++index);
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Name(@"Original learning start date").Index(++index);
            Map(m => m.LearningDelivery.LearnStartDate).Name(@"Learning start date").Index(++index);
            Map(m => m.LearningDelivery.LearnPlanEndDate).Name(@"Learning planned end date").Index(++index);
            Map(m => m.LearningDelivery.CompStatus).Name(@"Completion status").Index(++index);
            Map(m => m.LearningDelivery.LearnActEndDateNullable).Name(@"Learning actual end date").Index(++index);
            Map(m => m.LearningDelivery.OutcomeNullable).Name(@"Outcome").Index(++index);
            Map(m => m.LearningDelivery.AchDateNullable).Name(@"Achievement date").Index(++index);
            Map(m => m.LearningDeliveryFAMs.SOF).Name(@"Learning delivery funding and monitoring type - source of funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.EEF).Name(@"Learning delivery funding and monitoring type - eligibility for enhanced apprenticeship funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_Highest).Name(@"Learning delivery funding and monitoring type - learning support funding (highest applicable)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_EarliestDateFrom).Name(@"Learning delivery funding and monitoring - LSF date applies from (earliest) ").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_LatestDateTo).Name(@"Learning delivery funding and monitoring - LSF date applies to (latest)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM1).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (A)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM2).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (B)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM3).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (C)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM4).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (D)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM5).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (E)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM6).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (F)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.RES).Name(@"Learning delivery funding and monitoring type - restart indicator").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.A).Name(@"Provider specified delivery monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.B).Name(@"Provider specified delivery monitoring (B)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.C).Name(@"Provider specified delivery monitoring (C)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.D).Name(@"Provider specified delivery monitoring (D)").Index(++index);
            Map(m => m.LearningDelivery.EPAOrgID).Name(@"End point assessment organisation").Index(++index);
            Map(m => m.LearningDelivery.PartnerUKPRNNullable).Name(@"Sub contracted or partnership UKPRN").Index(++index);
            Map(m => m.LearningDelivery.DelLocPostCode).Name(@"Delivery location postcode").Index(++index);
            Map(m => m.Fm81LearningDelivery.CoreGovContCapApplicVal).Name(@"LARS maximum core government contribution (£)").Index(++index);
            Map(m => m.Fm81LearningDelivery.SmallBusApplicVal).Name(@"LARS small employer incentive (£)").Index(++index);
            Map(m => m.Fm81LearningDelivery.YoungAppApplicVal).Name(@"LARS 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.Fm81LearningDelivery.AchievementApplicVal).Name(@"LARS achievement incentive (£)").Index(++index);
            Map(m => m.Fm81LearningDelivery.ApplicFundValDate).Name(@"Applicable funding value date").Index(++index);
            Map(m => m.Fm81LearningDelivery.FundLine).Name(@"Funding line type").Index(++index);
            Map(m => m.Fm81LearningDelivery.EmpIdFirstDayStandard).Name(@"Employer identifier on first day of standard").Index(++index);
            Map(m => m.Fm81LearningDelivery.EmpIdSmallBusDate).Name(@"Employer identifier on small employer threshold date").Index(++index);
            Map(m => m.Fm81LearningDelivery.EmpIdFirstYoungAppDate).Name(@"Employer identifier on first 16-18 threshold date").Index(++index);
            Map(m => m.Fm81LearningDelivery.EmpIdSecondYoungAppDate).Name(@"Employer identifier on second 16-18 threshold date").Index(++index);
            Map(m => m.Fm81LearningDelivery.EmpIdAchDate).Name(@"Employer identifier on achievement date").Index(++index);
            Map(m => m.Fm81LearningDelivery.MathEngLSFFundStart).Name(@"Start indicator for maths, English and learning support").Index(++index);
            Map(m => m.Fm81LearningDelivery.AgeStandardStart).Name(@"Age at start of standard").Index(++index);
            Map(m => m.Fm81LearningDelivery.YoungAppEligible).Name(@"Eligible for 16-18 year-old apprentice incentive").Index(++index);
            Map(m => m.Fm81LearningDelivery.SmallBusEligible).Name(@"Eligible for small employer incentive").Index(++index);
            Map(m => m.Fm81LearningDelivery.AchApplicDate).Name(@"Applicable achievement date").Index(++index);
            Map(m => m.AppFinRecord.LatestTotalNegotiatedPrice1).Name(@"Latest total negotiated price (TNP) 1 (£)").Index(++index);
            Map(m => m.AppFinRecord.LatestTotalNegotiatedPrice2).Name(@"Latest total negotiated price (TNP) 2 (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfPmrsBeforeFundingYear).Name(@"Sum of PMRs before this funding year (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfAugustPmrs).Name(@"Sum of August payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.CoreGovContPayment).Name(@"August core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.MathEngOnProgPayment).Name(@"August maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.MathEngBalPayment).Name(@"August maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.LearnSuppFundCash).Name(@"August learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.SmallBusPayment).Name(@"August small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.YoungAppPayment).Name(@"August 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.August.AchievePayment).Name(@"August achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfSeptemberPmrs).Name(@"Sum of September payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.CoreGovContPayment).Name(@"September core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.MathEngOnProgPayment).Name(@"September maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.MathEngBalPayment).Name(@"September maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.LearnSuppFundCash).Name(@"September learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.SmallBusPayment).Name(@"September small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.YoungAppPayment).Name(@"September 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.September.AchievePayment).Name(@"September achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfOctoberPmrs).Name(@"Sum of October payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.CoreGovContPayment).Name(@"October core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.MathEngOnProgPayment).Name(@"October maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.MathEngBalPayment).Name(@"October maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.LearnSuppFundCash).Name(@"October learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.SmallBusPayment).Name(@"October small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.YoungAppPayment).Name(@"October 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.October.AchievePayment).Name(@"October achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfNovemberPmrs).Name(@"Sum of November payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.CoreGovContPayment).Name(@"November core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.MathEngOnProgPayment).Name(@"November maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.MathEngBalPayment).Name(@"November maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.LearnSuppFundCash).Name(@"November learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.SmallBusPayment).Name(@"November small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.YoungAppPayment).Name(@"November 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.November.AchievePayment).Name(@"November achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfDecemberPmrs).Name(@"Sum of December payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.CoreGovContPayment).Name(@"December core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.MathEngOnProgPayment).Name(@"December maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.MathEngBalPayment).Name(@"December maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.LearnSuppFundCash).Name(@"December learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.SmallBusPayment).Name(@"December small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.YoungAppPayment).Name(@"December 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.December.AchievePayment).Name(@"December achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfJanuaryPmrs).Name(@"Sum of January payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.CoreGovContPayment).Name(@"January core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.MathEngOnProgPayment).Name(@"January maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.MathEngBalPayment).Name(@"January maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.LearnSuppFundCash).Name(@"January learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.SmallBusPayment).Name(@"January small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.YoungAppPayment).Name(@"January 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.January.AchievePayment).Name(@"January achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfFebruaryPmrs).Name(@"Sum of February payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.CoreGovContPayment).Name(@"February core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.MathEngOnProgPayment).Name(@"February maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.MathEngBalPayment).Name(@"February maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.LearnSuppFundCash).Name(@"February learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.SmallBusPayment).Name(@"February small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.YoungAppPayment).Name(@"February 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.February.AchievePayment).Name(@"February achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfMarchPmrs).Name(@"Sum of March payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.CoreGovContPayment).Name(@"March core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.MathEngOnProgPayment).Name(@"March maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.MathEngBalPayment).Name(@"March maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.LearnSuppFundCash).Name(@"March learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.SmallBusPayment).Name(@"March small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.YoungAppPayment).Name(@"March 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.March.AchievePayment).Name(@"March achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfAprilPmrs).Name(@"Sum of April payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.CoreGovContPayment).Name(@"April core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.MathEngOnProgPayment).Name(@"April maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.MathEngBalPayment).Name(@"April maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.LearnSuppFundCash).Name(@"April learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.SmallBusPayment).Name(@"April small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.YoungAppPayment).Name(@"April 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.April.AchievePayment).Name(@"April achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfMayPmrs).Name(@"Sum of May payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.CoreGovContPayment).Name(@"May core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.MathEngOnProgPayment).Name(@"May maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.MathEngBalPayment).Name(@"May maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.LearnSuppFundCash).Name(@"May learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.SmallBusPayment).Name(@"May small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.YoungAppPayment).Name(@"May 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.May.AchievePayment).Name(@"May achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfJunePmrs).Name(@"Sum of June payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.CoreGovContPayment).Name(@"June core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.MathEngOnProgPayment).Name(@"June maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.MathEngBalPayment).Name(@"June maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.LearnSuppFundCash).Name(@"June learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.SmallBusPayment).Name(@"June small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.YoungAppPayment).Name(@"June 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.June.AchievePayment).Name(@"June achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.SumOfJulyPmrs).Name(@"Sum of July payment records (PMRs) (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.CoreGovContPayment).Name(@"July core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.MathEngOnProgPayment).Name(@"July maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.MathEngBalPayment).Name(@"July maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.LearnSuppFundCash).Name(@"July learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.SmallBusPayment).Name(@"July small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.YoungAppPayment).Name(@"July 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.July.AchievePayment).Name(@"July achievement incentive (£)").Index(++index);
            Map(m => m.AppFinRecord.PmrsTotal).Name(@"Total payment records (PMRs) for this funding year (£)").Index(++index);
            Map(m => m.PeriodisedValues.CoreGovContPaymentTotal).Name(@"Total core government contribution (£)").Index(++index);
            Map(m => m.PeriodisedValues.MathEngOnProgPaymentTotal).Name(@"Total maths and English on-programme earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.MathEngBalPaymentTotal).Name(@"Total maths and English balancing earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.LearnSuppFundCashTotal).Name(@"Total learning support earned cash (£)").Index(++index);
            Map(m => m.PeriodisedValues.SmallBusPaymentTotal).Name(@"Total small employer incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.YoungAppPaymentTotal).Name(@"Total 16-18 year-old apprentice incentive (£)").Index(++index);
            Map(m => m.PeriodisedValues.AchievePaymentTotal).Name(@"Total achievement incentive (£)").Index(++index);
            Map().Name(@"OFFICIAL - SENSITIVE").Constant(string.Empty).Index(++index);
        }
    }
}
