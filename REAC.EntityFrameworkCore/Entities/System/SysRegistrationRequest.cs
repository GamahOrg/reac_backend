using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysRegistrationRequest : FullAuditedBaseEntity<Guid>
{
    [Comment("Number of the identity")]
    public string IdentityNumber { get; set; } = null!;

    [Comment("Mobile phone number")]
    public string Mobile { get; set; } = null!;

    [Comment("Email address")]
    public string Email { get; set; } = null!;

    [Comment("Number of the identity")]
    public string? Cranumber { get; set; }

    [Comment("Number of the identity")]
    public string? Facility700Number { get; set; }

    [Comment("Unique identifier of the source citizen")]
    public Guid SourceCitizenId { get; set; }

    [Comment("Unique identifier of the source citizen")]
    public Guid? CitizenId { get; set; }

    [Comment("Date when the registration request was submitted")]
    public DateTime RequestedOn { get; set; }

    [Comment("Description for Is Registration Done")]
    public bool IsRegistrationDone { get; set; }

    [Comment("Date when the registration was completed")]
    public DateTime? RegistrationDoneOn { get; set; }

    public virtual Citizen? Citizen { get; set; }

    public virtual ICollection<LitigationRequestContract> SecondPartyLitigationRequestContracts { get; set; } = new List<LitigationRequestContract>();


    public virtual Citizen SourceCitizen { get; set; } = null!;
}

