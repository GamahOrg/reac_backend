using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class FacilityCreateUpdateDto
{
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public Guid AccountId { get; set; }

    public string Facility700Number { get; set; } = null!;

    public string CommercialRegisterNumber { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? City { get; set; } = null!;

    public string? District { get; set; } = null!;

    public string? Street { get; set; } = null!;

    public string? PostalCode { get; set; }

    public string? SubNumber { get; set; }

    public string? BuildingNumber { get; set; } = null!;

    public string? UnitNumber { get; set; } = null!;

    public virtual CitizenCreateUpdateDto Account { get; set; } = null!;

    public virtual ICollection<FacilityDelegatorCreateUpdateDto> FacilityDelegators { get; set; } = new List<FacilityDelegatorCreateUpdateDto>();

    public virtual ICollection<LitigationRequestContractCreateUpdateDto> LitigationRequestContractFirstPartyFacilities { get; set; } = new List<LitigationRequestContractCreateUpdateDto>();

    public virtual ICollection<LitigationRequestContractCreateUpdateDto> LitigationRequestContractSecondPartyFacilities { get; set; } = new List<LitigationRequestContractCreateUpdateDto>();
}
