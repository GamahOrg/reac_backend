using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysRegistrationRequest : FullAuditedBaseEntity<Guid>
{
    public string IdentityNumber { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Cranumber { get; set; }

    public string? Facility700Number { get; set; }

    public Guid SourceCitizenId { get; set; }

    public Guid? CitizenId { get; set; }

    public DateTime RequestedOn { get; set; }

    public bool IsRegistrationDone { get; set; }

    public DateTime? RegistrationDoneOn { get; set; }

    public virtual Citizen? Citizen { get; set; }

    public virtual ICollection<LitigationRequestContract> SecondPartyLitigationRequestContracts { get; set; } = new List<LitigationRequestContract>();


    public virtual Citizen SourceCitizen { get; set; } = null!;
}
