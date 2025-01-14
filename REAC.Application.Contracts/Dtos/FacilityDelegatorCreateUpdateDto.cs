using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class FacilityDelegatorCreateUpdateDto
{
    public Guid Id { get; set; }

    public string? DelegateNumber { get; set; }

    public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

    public Guid? FacilityId { get; set; }

    public Guid DelegatorCitizenId { get; set; }

    public Guid DelegationFileId { get; set; }

    public bool IsActive { get; set; }

    public virtual CitizenCreateUpdateDto DelegatorCitizen { get; set; } = null!;

    public virtual FacilityCreateUpdateDto? Facility { get; set; }
}
