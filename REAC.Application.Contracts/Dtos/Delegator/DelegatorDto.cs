using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Delegator
{

    public class DelegatorDto : BaseEntity<Guid>
    {

        public string? DelegateNumber { get; set; }

        public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

        public string IdentityNumber { get; set; } = null!;

        //Lawyer
        public string? LawyerLicenseNumber { get; set; }
        public DateTime? LawyerLicenseExpirationDate { get; set; }
        public Guid? LawLicenseFileId { get; set; }

        public Guid? FacilityId { get; set; }

        public Guid? DelegatorCitizenId { get; set; }

        public Guid DelegationFileId { get; set; }

        public bool IsActive { get; set; }

    }

    public class LawyerDelegatorDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; }
        public string LawyerLicenseNumber { get; set; }
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public string? DelegateNumber { get; set; }

        public DelegationRelationshipType? DelegationRelationshipTypeId { get; set; }

        public Guid? DelegatorCitizenId { get; set; }


        public Guid? DelegationFileId { get; set; }

        public Guid? FacilityId { get; set; }
        public bool IsFacility { get; set; }


    }
}
