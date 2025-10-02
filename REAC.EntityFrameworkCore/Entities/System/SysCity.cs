using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysCity : BaseEntity<Guid>
    {
        [Comment("Name of the city")]
        public string CityName { get; set; } = null!;
        [Comment("Unique identifier of the area")]
        public Guid AreaId { get; set; }
        public SysArea Area { get; set; } = null!;
        public ICollection<LitigationRequest> LitigationRequests { get; set; }
        public ICollection<Citizen> Citizens { get; set; }

        public virtual ICollection<SysCityTranslation> Translations { get; set; } = new List<SysCityTranslation>();

    }

}

