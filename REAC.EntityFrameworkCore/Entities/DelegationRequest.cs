using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class DelegationRequest : BaseEntity<Guid>
{
    public LitigationRequestParty DelegationParty { get; set; }

    public Guid? DelegatorId { get; set; }
    public Guid? LitigationRequestId { get; set; }
    public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }

    public DateTime RequestedOn { get; set; }

    public bool? IsApproved { get; set; }
    public bool? IsActive { get; set; }

    public DateTime? ApprovedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
    public string? Reason { get; set; }

    [ForeignKey(nameof(DelegatorId))]
    public virtual Delegator? Delegator { get; set; }
    
    [ForeignKey(nameof(LitigationRequestId))]
    public virtual LitigationRequest? LitigationRequest { get; set; }

    public virtual SysLitigationRequestOwnerType LitigationRequestOwnerType { get; set; } = null!;


}
