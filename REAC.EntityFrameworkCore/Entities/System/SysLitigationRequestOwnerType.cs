using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestOwnerType : BaseEntity<LitigationRequestOwnerType>
{
    /// <summary>
    /// مقدم عن نفسه – مفوض
    /// </summary>
    [Comment("Description for Litigation Request Owner Type")]
    public string LitigationRequestOwnerType { get; set; } = null!;

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<DelegationRequest> DelegationRequests { get; set; } = new List<DelegationRequest>();

    public virtual ICollection<SysLitigationRequestOwnerTypeTranslation> Translations { get; set; } = new List<SysLitigationRequestOwnerTypeTranslation>();

}

