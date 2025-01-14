using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysExemptionReductionCategory : BaseEntity<Guid>
{
    public string ExemptionReductionCategoryName { get; set; } = null!;

    public virtual ICollection<SysExemptionReductionCategoryTranslation> Translations { get; set; } = new List<SysExemptionReductionCategoryTranslation>();

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();

}
