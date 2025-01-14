using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Delegator
{

    public class DelegatorFullDataDto : BaseEntity<Guid>
    {
        public string? DelegateNumber { get; set; }

        public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

        public Guid? CitizenId { get; set; }
        public string IdentityNumber { get; set; } = null!;

        //Lawyer
        public string? LawyerLicenseNumber { get; set; }
        public DateTime? LawyerLicenseExpirationDate { get; set; }
        public Guid? LawLicenseFileId { get; set; }

        public Guid? FacilityId { get; set; }

        public Guid? DelegatorCitizenId { get; set; }

        public Guid DelegationFileId { get; set; }

        public bool IsActive { get; set; }

        public virtual CitizenShortDto DelegatorCitizen { get; set; } = null!;
        public virtual CitizenShortDto? Citizen { get; set; }


        public virtual SysDelegationRelationshipType SysDelegationRelationshipType { get; set; } = null!;

        public virtual SysFileManagerCreateUpdateDto DelegationFile { get; set; } = null!;

        public virtual FacilityShortDto? Facility { get; set; }

    }
}
