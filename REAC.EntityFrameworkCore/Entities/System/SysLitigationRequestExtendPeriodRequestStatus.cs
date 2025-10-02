using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestExtendPeriodRequestStatus : BaseEntity<LitigationRequestExtendPeriodRequestStatus>
{
    [Comment("Status of the name")]
    public string StatusName { get; set; } = null!;

    public virtual ICollection<SysLitigationRequestExtendPeriodRequestStatusTranslation> Translations { get; set; } = new List<SysLitigationRequestExtendPeriodRequestStatusTranslation>();

    public virtual ICollection<LitigationRequestExtendPeriodRequest> LitigationRequestExtendPeriodRequests { get; set; } = new List<LitigationRequestExtendPeriodRequest>();
}
