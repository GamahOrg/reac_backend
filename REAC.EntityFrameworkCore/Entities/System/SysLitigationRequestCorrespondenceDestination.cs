using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestCorrespondenceDestination : BaseEntity<LitigationRequestCorrespondenceDestination>
{
    [Comment("Description for Destination")]
    public string Destination { get; set; } = null!;


    public virtual ICollection<SysLitigationRequestCorrespondenceDestinationTranslation> Translations { get; set; } = new List<SysLitigationRequestCorrespondenceDestinationTranslation>();

    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();
}
