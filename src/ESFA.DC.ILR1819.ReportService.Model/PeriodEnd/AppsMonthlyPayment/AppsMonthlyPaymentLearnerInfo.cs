﻿using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AppsMonthlyPaymentLearnerInfo
    {
        public string LearnRefNumber { get; set; }

        public string CampId { get; set; }

        public ICollection<AppsMonthlyPaymentLearningDeliveryInfo> LearningDeliveries { get; set; }

        public ICollection<AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo> ProviderSpecLearnerMonitorings { get; set; }

        public ICollection<AppsMonthlyPaymentProviderSpecDeliveryMonitoringInfo> ProviderSpecDeliveryMonitorings { get; set; }

        public ICollection<AppsMonthlyPaymentLearningDeliveryFAMInfo> LearningDeliveryFams { get; set; }

    }
}