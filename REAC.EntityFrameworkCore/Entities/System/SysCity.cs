using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysCity : BaseEntity<Guid>
    {
        public string CityName { get; set; } = null!;
        public Guid AreaId { get; set; }
        public SysArea Area { get; set; } = null!;
        public ICollection<LitigationRequest> LitigationRequests { get; set; }
        public ICollection<Citizen> Citizens { get; set; }

        public virtual ICollection<SysCityTranslation> Translations { get; set; } = new List<SysCityTranslation>();

    }

}
