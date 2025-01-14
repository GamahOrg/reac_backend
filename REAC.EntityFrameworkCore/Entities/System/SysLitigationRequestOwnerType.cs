using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestOwnerType : BaseEntity<LitigationRequestOwnerType>
{
    /// <summary>
    /// مقدم عن نفسه – مفوض
    /// </summary>
    public string LitigationRequestOwnerType { get; set; } = null!;

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();

    public virtual ICollection<SysLitigationRequestOwnerTypeTranslation> Translations { get; set; } = new List<SysLitigationRequestOwnerTypeTranslation>();

}
