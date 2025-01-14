using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestExtendPeriodRequestStatus : BaseEntity<LitigationRequestExtendPeriodRequestStatus>
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<SysLitigationRequestExtendPeriodRequestStatusTranslation> Translations { get; set; } = new List<SysLitigationRequestExtendPeriodRequestStatusTranslation>();

    public virtual ICollection<LitigationRequestExtendPeriodRequest> LitigationRequestExtendPeriodRequests { get; set; } = new List<LitigationRequestExtendPeriodRequest>();
}
