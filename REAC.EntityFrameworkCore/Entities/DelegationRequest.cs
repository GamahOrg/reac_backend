using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class DelegationRequest : BaseEntity<Guid>
{
    [Comment("Party involved in the delegation (plaintiff or defendant)")]
    public LitigationRequestParty DelegationParty { get; set; }

    [Comment("Unique identifier of the delegator")]
    public Guid? DelegatorId { get; set; }
    [Comment("Unique identifier of the litigation request")]
    public Guid? LitigationRequestId { get; set; }
    [Comment("Unique identifier of the litigation request owner type")]
    public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }

    [Comment("Date when the delegation request was submitted")]
    public DateTime RequestedOn { get; set; }

    [Comment("Indicates whether the delegation request has been approved")]
    public bool? IsApproved { get; set; }
    [Comment("Indicates whether the record is active")]
    public bool? IsActive { get; set; }

    [Comment("Date when the delegation request was approved")]
    public DateTime? ApprovedOn { get; set; }
    [Comment("Date and time when the record was last modified")]
    public DateTime? ModifiedOn { get; set; }
    [Comment("Unique identifier of the user who last modified this record")]
    public Guid? ModifiedBy { get; set; }
    [Comment("Reason for the delegation request or modification")]
    public string? Reason { get; set; }

    [ForeignKey(nameof(DelegatorId))]
    public virtual Delegator? Delegator { get; set; }
    
    [ForeignKey(nameof(LitigationRequestId))]
    public virtual LitigationRequest? LitigationRequest { get; set; }

    public virtual SysLitigationRequestOwnerType LitigationRequestOwnerType { get; set; } = null!;


}

