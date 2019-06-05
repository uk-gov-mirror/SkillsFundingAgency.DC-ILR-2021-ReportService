﻿namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public sealed class ServiceBusOptions
    {
        public string AuditQueueName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string ReportingSubscriptionName { get; set; }
    }
}
