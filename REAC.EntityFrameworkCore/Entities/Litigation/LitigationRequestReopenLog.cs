using System.ComponentModel.DataAnnotations.Schema;
using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public class LitigationRequestReopenLog : BaseEntity<Guid>
{
    [Comment("Foreign key to the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Reason for reopening the request - max 500 chars")]
    public string ReopenReason { get; set; } = null!;

    [Comment("Internal status before cancellation/rejection")]
    public RequestStatus PreviousInternalStatusId { get; set; }

    [Comment("Internal status after reopening")]
    public RequestStatus NewInternalStatusId { get; set; }

    [Comment("Stage before cancellation/rejection")]
    public LitigationRequestStage? PreviousStage { get; set; }

    [Comment("Stage after reopening")]
    public LitigationRequestStage? NewStage { get; set; }

    [Comment("SubStage before cancellation/rejection")]
    public LitigationRequestSubStage? PreviousSubStage { get; set; }

    [Comment("SubStage after reopening")]
    public LitigationRequestSubStage? NewSubStage { get; set; }

    [Comment("Date when the request was reopened")]
    public DateTime ReopenedOn { get; set; }

    [Comment("Employee who performed the reopen action")]
    public Guid? ReopenedBy { get; set; }

    public DateTime CreatedOn { get; set; }
    public Guid? CreatedBy { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    [ForeignKey(nameof(ReopenedBy))]
    public virtual SysEmployee? ReopenedByEmployee { get; set; }
}
