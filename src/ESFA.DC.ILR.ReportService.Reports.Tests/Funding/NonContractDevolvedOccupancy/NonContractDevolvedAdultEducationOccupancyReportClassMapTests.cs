﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractDevolvedOccupancy
{
    public class NonContractDevolvedAdultEducationOccupancyReportClassMapTests
    {
        [Fact]
        public void Map_Columns()
        {
            var orderedColumns = new string[]
            {
                "Learner reference number",
                "Unique learner number",
                "Family name",
                "Given names",
                "Date of birth",
                "Ethnicity",
                "Sex",
                "LLDD and health problem",
                "Prior attainment",
                "Postcode prior to enrolment",
                "Pre-merger UKPRN",
                "Campus identifier",
                "Provider specified learner monitoring (A)",
                "Provider specified learner monitoring (B)",
                "Aim sequence number",
                "Learning aim reference",
                "Learning aim title",
                "Software supplier aim identifier",
                "Applicable funding rate from ESOL hours",
                "Applicable funding rate",
                "Applicable programme weighting",
                "Aim value",
                "Notional NVQ level",
                "Entitlement category (level 2 or 3)",
                "Tier 2 sector subject area",
                "Tier 2 sector subject area name",
                "Funding model",
                "Funding adjustment for prior learning",
                "Other funding adjustment",
                "Original learning start date",
                "Learning start date",
                "Learning planned end date",
                "Completion status",
                "Learning actual end date",
                "Withdrawal reason",
                "Outcome",
                "Additional delivery hours",
                "Learning start date postcode",
                "Local authority code",
                "Applicable area from source of funding",
                "Learning delivery funding and monitoring type - source of funding",
                "Learning delivery funding and monitoring type - full or co funding indicator ",
                "Learning delivery funding and monitoring type - learning support funding (highest applicable)",
                "Learning delivery funding and monitoring type - LSF date applies from (earliest)",
                "Learning delivery funding and monitoring type - LSF date applies to (latest)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (A)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (B)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (C)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (D)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (E)",
                "Learning delivery funding and monitoring type - learning delivery monitoring (F)",
                "Learning delivery funding and monitoring type - devolved area monitoring (A)",
                "Learning delivery funding and monitoring type - devolved area monitoring (B)",
                "Learning delivery funding and monitoring type - devolved area monitoring (C)",
                "Learning delivery funding and monitoring type - devolved area monitoring (D)",
                "Learning delivery funding and monitoring type - devolved area monitoring (E)",
                "Learning delivery funding and monitoring type - devolved area monitoring (F)",
                "Learning delivery funding and monitoring type - restart indicator",
                "Learning delivery funding and monitoring type - Community Learning provision type",
                "Learning delivery funding and monitoring type - household situation (A)",
                "Learning delivery funding and monitoring type - household situation (B)",
                "Provider specified delivery monitoring (A)",
                "Provider specified delivery monitoring (B)",
                "Provider specified delivery monitoring (C)",
                "Provider specified delivery monitoring (D)",
                "Employment status",
                "Employment status monitoring type - employment intensity indicator",
                "Employment status monitoring type - benefit status indicator",
                "Funding line type",
                "Start for funding purposes",
                "Planned number of on programme instalments",
                "Achievement element (potential or actual earned cash)",
                "Achievement percentage (aggregated maximum value)",
                "Non-public-funded contribution",
                "Capping factor",
                "Sub contracted or partnership UKPRN",
                "Sub contracted or partnership UKPRN name",
                "Delivery location postcode",
                "Area uplift",
                "Disadvantage uplift",
                "Date used for uplifts and other lookups",
                "August on programme earned cash",
                "August balancing payment earned cash",
                "August aim achievement earned cash",
                "August job outcome earned cash",
                "August learning support earned cash",
                "September on programme earned cash",
                "September balancing payment earned cash",
                "September aim achievement earned cash",
                "September job outcome earned cash",
                "September learning support earned cash",
                "October on programme earned cash",
                "October balancing payment earned cash",
                "October aim achievement earned cash",
                "October job outcome earned cash",
                "October learning support earned cash",
                "November on programme earned cash",
                "November balancing payment earned cash",
                "November aim achievement earned cash",
                "November job outcome earned cash",
                "November learning support earned cash",
                "December on programme earned cash",
                "December balancing payment earned cash",
                "December aim achievement earned cash",
                "December job outcome earned cash",
                "December learning support earned cash",
                "January on programme earned cash",
                "January balancing payment earned cash",
                "January aim achievement earned cash",
                "January job outcome earned cash",
                "January learning support earned cash",
                "February on programme earned cash",
                "February balancing payment earned cash",
                "February aim achievement earned cash",
                "February job outcome earned cash",
                "February learning support earned cash",
                "March on programme earned cash",
                "March balancing payment earned cash",
                "March aim achievement earned cash",
                "March job outcome earned cash",
                "March learning support earned cash",
                "April on programme earned cash",
                "April balancing payment earned cash",
                "April aim achievement earned cash",
                "April job outcome earned cash",
                "April learning support earned cash",
                "May on programme earned cash",
                "May balancing payment earned cash",
                "May aim achievement earned cash",
                "May job outcome earned cash",
                "May learning support earned cash",
                "June on programme earned cash",
                "June balancing payment earned cash",
                "June aim achievement earned cash",
                "June job outcome earned cash",
                "June learning support earned cash",
                "July on programme earned cash",
                "July balancing payment earned cash",
                "July aim achievement earned cash",
                "July job outcome earned cash",
                "July learning support earned cash",
                "Total on programme earned cash",
                "Total balancing payment earned cash",
                "Total aim achievement earned cash",
                "Total job outcome earned cash",
                "Total learning support earned cash",
                "Total earned cash",
                "OFFICIAL - SENSITIVE",
            };

            var input = new List<NonContractDevolvedAdultEducationOccupancyReportModel>();

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<NonContractDevolvedAdultEducationOccupancyReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        var header = csvReader.Context.HeaderRecord;
                        
                        header.Should().ContainInOrder(orderedColumns);
                        
                        header.Should().HaveCount(148);
                    }
                }
            }
        }

        [Fact]
        public void ClassMap_Model()
        {
            var input = new List<NonContractDevolvedAdultEducationOccupancyReportModel>()
            {
                new NonContractDevolvedAdultEducationOccupancyReportModel()
                {
                    Learner = new TestLearner()
                    {
                        LearnRefNumber = "Test"
                    },
                    LearningDelivery = new TestLearningDelivery(),
                    LarsLearningDelivery = new LARSLearningDelivery(),
                    Fm35LearningDelivery = new LearningDeliveryValue(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                }
            };

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8096, true))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<NonContractDevolvedAdultEducationOccupancyReportClassMap>();

                        csvWriter.WriteRecords(input);
                    }
                }

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(streamReader))
                    {
                        var output = csvReader.GetRecords<dynamic>().ToList();

                        (output[0] as IDictionary<string, object>).Values.ToArray()[0].Should().Be("Test");
                    }
                }
            }
        }
    }
}
