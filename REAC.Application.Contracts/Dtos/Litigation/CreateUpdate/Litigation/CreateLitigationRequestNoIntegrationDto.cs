using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class CreateLitigationRequestNoIntegrationDto
    {
        public string key { get; set; } = null!;
        public string SubKey { get; set; } = null!;
        public string? AssociationName { get; set; }
        public LitigationRequestContractType ContractType { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
        public Guid? IncorporationContractFileId { get; set; }
        public string? FirstPartyCapacity { get; set; }
        public string? ContractAddress { get; set; }
        public Guid? FirstPartyFacilityId { get; set; }
        public Guid? FirstPartyIndividualId { get; set; }
        public string? SecondPartyCapacity { get; set; }
        public int? SecondPartyCapacityId { get; set; }
        public string? SecondPartyNationalId { get; set; }
        public string? SecondPartyPhone { get; set; }
        public string? SecondPartyEmail { get; set; }
        public string? SecondPartyName { get; set; }
        public string? SecondPartyFacilityCommercialRegistrationNo { get; set; }
        public Guid? OtherCategories { get; set; }

        public LitigationRequestDelegationDataDto? litigationRequestDelegationDataDto { get; set; }
    }

    public class LitigationRequestDelegationDataDto
    {
        public DelegationRelationshipType? DelegationRelationshipTypeId { get; set; }

        public string? DelegationNumber { get; set; }
        public Guid? DelegationFileId { get; set; }
        public SysFileManagerCreateUpdateDto? DelegationFile { get; set; }
        public Guid? CommercialRegisterationFileId { get; set; }
        public SysFileManagerCreateUpdateDto? CommercialRegisterationFile { get; set; }
        public Guid? EstablishmentFileId { get; set; }
        public SysFileManagerCreateUpdateDto? EstablishmentFile { get; set; }
        public Guid? EstablishmentLetterFileId { get; set; }
        public SysFileManagerCreateUpdateDto? EstablishmentLetterFile { get; set; }
        public Guid? RepresentationFileId { get; set; }
        public SysFileManagerCreateUpdateDto? RepresentationFile { get; set; }
        public Guid? DecisionFileId { get; set; }
        public SysFileManagerCreateUpdateDto? DecisionFile { get; set; }
        public Guid? CustodyFileId { get; set; }
        public SysFileManagerCreateUpdateDto? CustodyFile { get; set; }

        public string? IndividualIdentityNumber {get; set;}
        public string? IndividualFullName {get; set;}
        public string? IndividualMobile {get; set;}
        public string? IndividualCapacity {get; set;}
        public string? IndividualNationality {get; set;}
        public string? IndividualEmail {get; set;}
        public string? IndividualNationalAddress {get; set;}
        public string? IndividualBirthdate {get; set;}


        public string? ArabicFacilityName {get; set;}
        public string? Facility700Number {get; set;}
        public string? FacilityCommercialRegisterationNumber {get; set;}

    }

}
