using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorClass : BaseEntity<Guid>
    {
        public string ArbitratorClassName { get; set; } = null!;
        public int YearsFrom { get; set; }
        public int YearsTo { get; set; }
        public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();

        public virtual ICollection<SysArbitratorClassTranslation> Translations { get; set; } = new List<SysArbitratorClassTranslation>();


    }
}
