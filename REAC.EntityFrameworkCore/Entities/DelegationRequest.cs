using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class DelegationRequest : BaseEntity<Guid>
{
    public IdentityType IdentityTypeId { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public DateTime? BirthDate { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public Guid SourceCitizenId { get; set; }

    public Guid? CitizenId { get; set; }

    public DateTime RequestedOn { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string DelegationNumber { get; set; } = null!;

    public Guid DelegationFileId { get; set; }

    public virtual SysFileManager DelegationFile { get; set; } = null!;

    public virtual SysIdentityType IdentityType { get; set; } = null!;
}
