﻿using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MCAGLA;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData
{
    public class ReferenceDataRoot
    {
        public MetaData.MetaData MetaDatas { get; set; }

        public IReadOnlyCollection<FcsContractAllocation> FCSContractAllocations { get; set; }

        public IReadOnlyCollection<LARSLearningDelivery> LARSLearningDeliveries { get; set; }

        public IReadOnlyCollection<LARSStandard> LARSStandards { get; set; }

        public IReadOnlyCollection<McaDevolvedContract> McaDevolvedContracts { get; set; }

        public IReadOnlyCollection<Organisation> Organisations { get; set; }

        public DevolvedPostcodes.DevolvedPostcodes DevolvedPostocdes { get; set; }

        public IReadOnlyCollection<Postcode> Postcodes { get; set; }
    }
}
