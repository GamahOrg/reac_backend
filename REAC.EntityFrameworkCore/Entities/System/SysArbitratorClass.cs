using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorClass : BaseEntity<Guid>
    {
        [Comment("Name of the arbitrator class")]
        public string ArbitratorClassName { get; set; } = null!;
        [Comment("Description for Years From")]
        public int YearsFrom { get; set; }
        [Comment("Description for Years To")]
        public int YearsTo { get; set; }
        public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();

        public virtual ICollection<SysArbitratorClassTranslation> Translations { get; set; } = new List<SysArbitratorClassTranslation>();


    }
}

