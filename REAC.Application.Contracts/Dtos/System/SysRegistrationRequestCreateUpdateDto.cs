using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysRegistrationRequestCreateUpdateDto : BaseEntity<Guid>
{
    public string IdentityNumber { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Cranumber { get; set; }

    public string? Facility700Number { get; set; }

    public Guid SourceAccountId { get; set; }

    public Guid? AccountId { get; set; }

    public DateTime RequestedOn { get; set; }

    public bool IsRegistrationDone { get; set; }

    public DateTime? RegistrationDoneOn { get; set; }

}
