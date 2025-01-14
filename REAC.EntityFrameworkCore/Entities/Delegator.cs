using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class Delegator : BaseEntity<Guid>
{
    public string? DelegateNumber { get; set; }

    public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

    public Guid? CitizenId { get; set; }

    public Guid? FacilityId { get; set; }

    public Guid DelegatorCitizenId { get; set; }

    public Guid? DelegationFileId { get; set; }

    public bool IsActive { get; set; }
    public virtual SysDelegationRelationshipType SysDelegationRelationshipType { get; set; } = null!;

    public virtual Citizen? Citizen { get; set; }

    public virtual Citizen DelegatorCitizen { get; set; } = null!;

    public virtual SysFileManager? DelegationFile { get; set; }

    public virtual Facility? Facility { get; set; }
}
