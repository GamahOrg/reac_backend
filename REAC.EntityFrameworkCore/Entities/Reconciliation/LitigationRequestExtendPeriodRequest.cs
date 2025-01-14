using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestExtendPeriodRequest : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public LitigationRequestExtendPeriodRequestType LitigationRequestExtendPeriodRequestTypeId { get; set; }

    public int RequiredDays { get; set; }

    public string RequestReason { get; set; } = null!;


    public string? Notes { get; set; }

    public LitigationRequestExtendPeriodRequestStatus? LitigationRequestExtendPeriodRequestStatusId { get; set; }


    public string? FinishStatusReason { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestExtendPeriodRequestFile> LitigationRequestExtendPeriodRequestFiles { get; set; } = new List<LitigationRequestExtendPeriodRequestFile>();

    public virtual SysLitigationRequestExtendPeriodRequestStatus? LitigationRequestExtendPeriodRequestStatus { get; set; }
}
