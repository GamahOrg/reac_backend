using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestExtendPeriodRequest : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Type of the period extension request")]
    public LitigationRequestExtendPeriodRequestType LitigationRequestExtendPeriodRequestTypeId { get; set; }

    [Comment("Number of additional days requested for the extension")]
    public int RequiredDays { get; set; }

    [Comment("Reason for requesting the period extension")]
    public string RequestReason { get; set; } = null!;


    [Comment("Additional notes about the period extension request")]
    public string? Notes { get; set; }

    [Comment("Status identifier")]
    public LitigationRequestExtendPeriodRequestStatus? LitigationRequestExtendPeriodRequestStatusId { get; set; }


    [Comment("Status of the reason")]
    public string? FinishStatusReason { get; set; }
    [Comment("Unique identifier of the user who created this request")]
    public Guid? CreatedBy { get; set; }
    [Comment("Date and time when the record was created")]
    public DateTime? CreatedOn { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestExtendPeriodRequestFile> LitigationRequestExtendPeriodRequestFiles { get; set; } = new List<LitigationRequestExtendPeriodRequestFile>();

    public virtual SysLitigationRequestExtendPeriodRequestStatus? LitigationRequestExtendPeriodRequestStatus { get; set; }
}


