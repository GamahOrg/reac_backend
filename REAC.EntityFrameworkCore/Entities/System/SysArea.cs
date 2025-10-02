using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArea : BaseEntity<Guid>
    {
        [Comment("Name of the area")]
        public string AreaName { get; set; } = null!;
        public List<SysCity>? Cities { get; set; }
        public ICollection<LitigationRequest>? LitigationRequests { get; set; }
        public ICollection<Citizen>? Citizens { get; set; }
        public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();

        public virtual ICollection<SysAreaTranslation> Translations { get; set; } = new List<SysAreaTranslation>();
    }
}

