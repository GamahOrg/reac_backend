using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities;

public partial class Facility : BaseEntity<Guid>
{
    public string? Code { get; set; }

    public Guid CitizenId { get; set; }

    public string? CommercialRegisterNumber { get; set; }

    public string Facility700Number { get; set; } = null!;

    public string ArabicFacilityName { get; set; } = null!;

    public string? EnglishFacilityName { get; set; } = null!;


    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? City { get; set; } = null!;

    public string? District { get; set; } = null!;

    public string? Street { get; set; } = null!;

    public string? PostalCode { get; set; }

    public string? SubNumber { get; set; }

    public string? BuildingNumber { get; set; } = null!;

    public string? UnitNumber { get; set; } = null!;

    public virtual Citizen Citizen { get; set; } = null!;

    public virtual ICollection<Delegator> Delegators { get; set; } = new List<Delegator>();

    public virtual ICollection<LitigationRequestContract> LitigationRequestContractFirstPartyFacilities { get; set; } = new List<LitigationRequestContract>();

    public virtual ICollection<LitigationRequestContract> LitigationRequestContractSecondPartyFacilities { get; set; } = new List<LitigationRequestContract>();
}
