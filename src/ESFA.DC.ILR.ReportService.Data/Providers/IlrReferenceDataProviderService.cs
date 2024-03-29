﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MCAGLA;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrReferenceDataProviderService : AbstractFileServiceProvider, IExternalDataProvider
    {
        public IlrReferenceDataProviderService(
            IFileService fileService, 
            IJsonSerializationService jsonSerializationService)
            : base(fileService, jsonSerializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var referenceData = await ProvideAsync<ReferenceDataService.Model.ReferenceDataRoot>(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken) as ReferenceDataService.Model.ReferenceDataRoot;

            return MapData(referenceData);
        }

        private ReferenceDataRoot MapData(ReferenceDataService.Model.ReferenceDataRoot root)
        {
            return new ReferenceDataRoot()
            {
                MetaDatas = MapData(root.MetaDatas),
                FCSContractAllocations = MapData(root.FCSContractAllocations),
                LARSLearningDeliveries = MapData(root.LARSLearningDeliveries),
                LARSStandards = MapData(root.LARSStandards),
                McaDevolvedContracts = MapData(root.McaDevolvedContracts),
                Organisations = MapData(root.Organisations),
                DevolvedPostocdes = MapData(root.DevolvedPostcodes),
                Postcodes = MapData(root.Postcodes)
            };
        }

        // Meta Data Mapper
        private MetaData MapData(ReferenceDataService.Model.MetaData.MetaData metaData)
        {
            return new MetaData()
            {
                DateGenerated = metaData.DateGenerated,
                ReferenceDataVersions = MapReferenceDataVersions(metaData.ReferenceDataVersions),
                ValidationErrors = metaData.ValidationErrors?.Select(MapValidationErrors).ToList(),
                CollectionDates = MapIlrCollectionDates(metaData.CollectionDates)
            };
        }

        private ReferenceDataVersion MapReferenceDataVersions(ReferenceDataService.Model.MetaData.ReferenceDataVersion metaDataReferenceDataVersions)
        {
            return new ReferenceDataVersion()
            {
                CoFVersion = MapCofVersion(metaDataReferenceDataVersions.CoFVersion),
                CampusIdentifierVersion = MapCampusIdentifierVersion(metaDataReferenceDataVersions.CampusIdentifierVersion),
                Employers = MapEmployersVersion(metaDataReferenceDataVersions.Employers),
                LarsVersion = MapLarsVersion(metaDataReferenceDataVersions.LarsVersion),
                OrganisationsVersion = MapOrganisationsVersion(metaDataReferenceDataVersions.OrganisationsVersion),
                PostcodesVersion = MapPostcodesVersion(metaDataReferenceDataVersions.PostcodesVersion),
                EasFileDetails = MapEas(metaDataReferenceDataVersions.EasFileDetails),
                DevolvedPostcodesVersion = MapDevolvedPostcodesVersion(metaDataReferenceDataVersions.DevolvedPostcodesVersion)
            };
        }

        private ValidationError MapValidationErrors(ReferenceDataService.Model.MetaData.ValidationError validationError)
        {
            return new ValidationError()
            {
                RuleName = validationError.RuleName,
                Severity = (ValidationError.SeverityLevel)validationError.Severity,
                Message = validationError.Message
            };
        }

        private IlrCollectionDates MapIlrCollectionDates(ReferenceDataService.Model.MetaData.CollectionDates.IlrCollectionDates ilrCollectionDates)
        {
            return new IlrCollectionDates()
            {
                CensusDates = ilrCollectionDates.CensusDates.Select(MapCensusDate).ToList(),
                ReturnPeriods = ilrCollectionDates.ReturnPeriods.Select(MapReturnPeriod).ToList()
            };
        }

        private CensusDate MapCensusDate(ReferenceDataService.Model.MetaData.CollectionDates.CensusDate censusDate)
        {
            return new CensusDate()
            {
                Period = censusDate.Period,
                Start = censusDate.Start,
                End = censusDate.End
            };
        }

        private ReturnPeriod MapReturnPeriod(ReferenceDataService.Model.MetaData.CollectionDates.ReturnPeriod returnPeriod)
        {
            return new ReturnPeriod
            {
                Name = returnPeriod.Name,
                Period = returnPeriod.Period,
                Start = returnPeriod.Start,
                End = returnPeriod.End
            };
        }

        private CoFVersion MapCofVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.CoFVersion coFVersion)
        {
            return new CoFVersion()
            {
                Version = coFVersion.Version
            };
        }

        private CampusIdentifierVersion MapCampusIdentifierVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.CampusIdentifierVersion campusIdentifierVersion)
        {
            return new CampusIdentifierVersion()
            {
                Version = campusIdentifierVersion.Version
            };
        }

        private EmployersVersion MapEmployersVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.EmployersVersion employers)
        {
            return new EmployersVersion()
            {
                Version = employers.Version
            };
        }

        private LarsVersion MapLarsVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.LarsVersion larsVersion)
        {
            return new LarsVersion()
            {
                Version = larsVersion.Version
            };
        }

        private OrganisationsVersion MapOrganisationsVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.OrganisationsVersion organisationsVersion)
        {
            return new OrganisationsVersion()
            {
                Version = organisationsVersion.Version
            };
        }

        private PostcodesVersion MapPostcodesVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.PostcodesVersion postcodesVersion)
        {
            return new PostcodesVersion()
            {
                Version = postcodesVersion.Version
            };
        }

        private EasFileDetails MapEas(ReferenceDataService.Model.MetaData.ReferenceDataVersions.EasFileDetails easFileDetails)
        {
            return new EasFileDetails()
            {
                FileName = easFileDetails?.FileName,
                UploadDateTime = easFileDetails?.UploadDateTime
            };
        }

        private DevolvedPostcodesVersion MapDevolvedPostcodesVersion(ReferenceDataService.Model.MetaData.ReferenceDataVersions.DevolvedPostcodesVersion devolvedPostcodesVersion)
        {
            return new DevolvedPostcodesVersion()
            {
                Version = devolvedPostcodesVersion.Version
            };
        }

        // FCS Data Mapper
        private IReadOnlyCollection<FcsContractAllocation> MapData(IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations)
        {
            return fcsContractAllocations?.Select(MapFcsContractAllocation).ToList();
        }

        private FcsContractAllocation MapFcsContractAllocation(ReferenceDataService.Model.FCS.FcsContractAllocation fcsContractAllocation)
        {
            return new FcsContractAllocation()
            {
                ContractAllocationNumber = fcsContractAllocation.ContractAllocationNumber,
                FundingStreamPeriodCode = fcsContractAllocation.FundingStreamPeriodCode,
            };
        }

        // LARS Delivery Data Mapper
        private IReadOnlyCollection<LARSLearningDelivery> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries?.Select(MapLarsLearningDelivery).ToList();
        }

        private LARSLearningDelivery MapLarsLearningDelivery(ReferenceDataService.Model.LARS.LARSLearningDelivery larsLearningDelivery)
        {
            return new LARSLearningDelivery()
            {
                LearnAimRef = larsLearningDelivery.LearnAimRef,
                LearnAimRefTitle = larsLearningDelivery.LearnAimRefTitle,
                LearnAimRefTypeDesc = larsLearningDelivery.LearnAimRefTypeDesc,
                NotionalNVQLevel = larsLearningDelivery.NotionalNVQLevel,
                NotionalNVQLevelv2 = larsLearningDelivery.NotionalNVQLevelv2,
                FrameworkCommonComponent = larsLearningDelivery.FrameworkCommonComponent,
                SectorSubjectAreaTier2 = larsLearningDelivery.SectorSubjectAreaTier2,
                SectorSubjectAreaTier2Desc = larsLearningDelivery.SectorSubjectAreaTier2Desc,
                LARSLearningDeliveryCategories = MapLarsLearningDeliveryCategories(larsLearningDelivery.LARSLearningDeliveryCategories),
                LARSFrameworks = MapLarsLearningDeliveryFrameworks(larsLearningDelivery.LARSFrameworks)
            };
        }

        private IReadOnlyCollection<LARSLearningDeliveryCategory> MapLarsLearningDeliveryCategories(IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDeliveryCategory> larsLearningDeliveryCategories)
        {
            return larsLearningDeliveryCategories?.Select(MapLarsLearningDeliveryCategory).ToList();
        }

        private LARSLearningDeliveryCategory MapLarsLearningDeliveryCategory(
            ReferenceDataService.Model.LARS.LARSLearningDeliveryCategory larsLearningDeliveryCategory)
        {
            return new LARSLearningDeliveryCategory
            {
                LearnAimRef = larsLearningDeliveryCategory.LearnAimRef,
                CategoryRef = larsLearningDeliveryCategory.CategoryRef,
                EffectiveFrom = larsLearningDeliveryCategory.EffectiveFrom,
                EffectiveTo = larsLearningDeliveryCategory.EffectiveTo
            };
        }

        private IReadOnlyCollection<LARSFramework> MapLarsLearningDeliveryFrameworks(IEnumerable<ReferenceDataService.Model.LARS.LARSFramework> larsFrameworks)
        {
            return larsFrameworks?.Select(MapLarsLearningDeliveryFramework).ToList();
        }

        private LARSFramework MapLarsLearningDeliveryFramework(ReferenceDataService.Model.LARS.LARSFramework larsFramework)
        {
            return new LARSFramework()
            {
                LARSFrameworkAim = new LARSFrameworkAim()
                {
                    LearnAimRef = larsFramework?.LARSFrameworkAim?.LearnAimRef,
                    FrameworkComponentType = larsFramework?.LARSFrameworkAim?.FrameworkComponentType
                }
            };
        }

        // LARS Standard Data Mapper
        private IReadOnlyCollection<LARSStandard> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSStandard> larsStandards)
        {
            return larsStandards?.Select(MapLarsStandard).ToList();
        }

        private LARSStandard MapLarsStandard(ReferenceDataService.Model.LARS.LARSStandard larsStandard)
        {
            return new LARSStandard()
            {
                StandardCode = larsStandard.StandardCode,
                NotionalEndLevel = larsStandard.NotionalEndLevel,
                EffectiveFrom = larsStandard.EffectiveFrom,
                EffectiveTo = larsStandard.EffectiveTo,
            };
        }

        // Organisation Data Mapper
        private IReadOnlyCollection<Organisation> MapData(IEnumerable<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            return organisations?.Select(MapOrganisation).ToList();
        }

        private Organisation MapOrganisation(ReferenceDataService.Model.Organisations.Organisation organisation)
        {
            return new Organisation()
            {
                UKPRN = organisation.UKPRN,
                Name = organisation.Name,
                OrganisationCoFRemovals = organisation.OrganisationCoFRemovals?.Select(MapOrganisationCoFRemoval).ToList(),
            };
        }

        private OrganisationCoFRemoval MapOrganisationCoFRemoval(ReferenceDataService.Model.Organisations.OrganisationCoFRemoval organisationCoFRemoval)
        {
            return new OrganisationCoFRemoval()
            {
                CoFRemoval = organisationCoFRemoval.CoFRemoval,
                EffectiveTo = organisationCoFRemoval.EffectiveTo,
                EffectiveFrom = organisationCoFRemoval.EffectiveFrom
            };
        }

        // Devolved Postcode Data Mapper
        private DevolvedPostcodes MapData(ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes postcodes)
        {
            return new DevolvedPostcodes()
            {
                McaGlaSofLookups = postcodes?.McaGlaSofLookups?.Select(MapMcaGlaSofLookup).ToList(),
            };
        }

        private McaGlaSofLookup MapMcaGlaSofLookup(ReferenceDataService.Model.PostcodesDevolution.McaGlaSofLookup mcaGlaSofLookup)
        {
            return new McaGlaSofLookup()
            {
                McaGlaFullName = mcaGlaSofLookup.McaGlaFullName,
                McaGlaShortCode = mcaGlaSofLookup.McaGlaShortCode,
                SofCode = mcaGlaSofLookup.SofCode,
                EffectiveTo = mcaGlaSofLookup.EffectiveTo,
                EffectiveFrom = mcaGlaSofLookup.EffectiveFrom
            };
        }

        // Mca data Mapper
        private IReadOnlyCollection<McaDevolvedContract> MapData(IEnumerable<ReferenceDataService.Model.McaContracts.McaDevolvedContract> mcaContracts)
        {
            return mcaContracts?.Select(MapMcaDevolvedContract).ToList();
        }

        private McaDevolvedContract MapMcaDevolvedContract(ReferenceDataService.Model.McaContracts.McaDevolvedContract contract)
        {
            return new McaDevolvedContract()
            {
                McaGlaShortCode = contract.McaGlaShortCode,
                Ukprn = contract.Ukprn,
                EffectiveFrom = contract.EffectiveFrom,
                EffectiveTo = contract.EffectiveTo
            };
        }

        //Map Postcodes data
        private IReadOnlyCollection<Postcode> MapData(IEnumerable<ReferenceDataService.Model.Postcodes.Postcode> postcodes)
        {
            return postcodes?.Select(MapPostcode).ToList();
        }

        private Postcode MapPostcode(ReferenceDataService.Model.Postcodes.Postcode postcode)
        {
            return  new Postcode()
            {
                PostCode = postcode.PostCode,
                ONSData = MapONSData(postcode.ONSData)
            };
        }

        private List<ONSData> MapONSData(IEnumerable<ReferenceDataService.Model.Postcodes.ONSData> OnsData)
        {
            return OnsData?.Select(MapONS).ToList();
        }

        private ONSData MapONS(ReferenceDataService.Model.Postcodes.ONSData ons)
        {
            return new ONSData()
            {
                LocalAuthority = ons.LocalAuthority,
                EffectiveFrom = ons.EffectiveFrom,
                EffectiveTo = ons.EffectiveTo
            };
        }
    }
}
