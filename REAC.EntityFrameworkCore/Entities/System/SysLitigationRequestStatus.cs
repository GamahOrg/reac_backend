using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestStatus : FullAuditedBaseEntity<RequestStatus>
{
    public string RequestStatus { get; set; } = null!;

    public virtual ICollection<LitigationRequest> LitigationRequestExternalStatuses { get; set; } = new List<LitigationRequest>();

    public virtual ICollection<LitigationRequest> LitigationRequestInternalStatuses { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<SysLitigationRequestStatusTranslation> Translations { get; set; } = new List<SysLitigationRequestStatusTranslation>();

}
