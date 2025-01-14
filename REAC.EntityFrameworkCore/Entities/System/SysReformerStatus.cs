using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysReformerStatus : BaseEntity<ReformerStatus>
    {
        public string StatusName { get; set; } = null!;
        public virtual ICollection<SysReformerStatusTranslation> Translations { get; set; } = new List<SysReformerStatusTranslation>();
        public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();
        public virtual ICollection<ReformerStopFreeUpDecision> ReformerStopFreeUpDecisions { get; set; } = new List<ReformerStopFreeUpDecision>();

    }

}
