using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysAreaDistances : BaseEntity<Guid>
    {
        public Guid SourceAreaId { get; set; }
        public Guid DestinationAreaId { get; set; }
        public decimal Distance { get; set; } 

    }
}
