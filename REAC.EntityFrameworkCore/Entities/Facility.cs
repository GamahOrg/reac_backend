using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities;

public partial class Facility : BaseEntity<Guid>
{
    [Comment("Facility code identifier")]
    public string? Code { get; set; }

    [Comment("Unique identifier of the citizen")]
    public Guid? CitizenId { get; set; }

    [Comment("Number of the commercial register")]
    public string? CommercialRegisterNumber { get; set; }

    [Comment("700 number identifier of the facility")]
    public string Facility700Number { get; set; } = null!;

    [Comment("Name of the arabic facility")]
    public string ArabicFacilityName { get; set; } = null!;

    [Comment("English name of the facility")]
    public string? EnglishFacilityName { get; set; } = null!;


    [Comment("Fax number")]
    public string? Fax { get; set; }

    [Comment("Email address")]
    public string? Email { get; set; }

    [Comment("City where the facility is located")]
    public string? City { get; set; } = null!;

    [Comment("District")]
    public string? District { get; set; } = null!;

    [Comment("Street address")]
    public string? Street { get; set; } = null!;

    [Comment("Postal code")]
    public string? PostalCode { get; set; }

    [Comment("Sub number of the facility address")]
    public string? SubNumber { get; set; }

    [Comment("Building number of the facility address")]
    public string? BuildingNumber { get; set; } = null!;

    [Comment("Unit number")]
    public string? UnitNumber { get; set; } = null!;

    public virtual Citizen Citizen { get; set; } = null!;

    public virtual ICollection<Delegator> Delegators { get; set; } = new List<Delegator>();

    public virtual ICollection<LitigationRequestContract> LitigationRequestContractFirstPartyFacilities { get; set; } = new List<LitigationRequestContract>();

    public virtual ICollection<LitigationRequestContract> LitigationRequestContractSecondPartyFacilities { get; set; } = new List<LitigationRequestContract>();

    [InverseProperty(nameof(LitigationRequest.ApplicantFacility))]
    public virtual ICollection<LitigationRequest> ApplicantFacility { get; set; } = new List<LitigationRequest>();
    
    [InverseProperty(nameof(LitigationRequest.DefendantFacility))]
    public virtual ICollection<LitigationRequest> DefendantFacility { get; set; } = new List<LitigationRequest>();
}

