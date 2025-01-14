using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestCorrespondenceDestination : BaseEntity<LitigationRequestCorrespondenceDestination>
{
    public string Destination { get; set; } = null!;


    public virtual ICollection<SysLitigationRequestCorrespondenceDestinationTranslation> Translations { get; set; } = new List<SysLitigationRequestCorrespondenceDestinationTranslation>();

    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();
}
