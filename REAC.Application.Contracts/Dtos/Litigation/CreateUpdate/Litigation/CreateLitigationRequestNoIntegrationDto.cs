using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class CreateLitigationRequestNoIntegrationDto
    {
        public string key { get; set; } = null!;
        public string SubKey { get; set; } = null!;
        public LitigationRequestContractType ContractType { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
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
    }

}
