using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysAreaDistances : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the source area")]
        public Guid SourceAreaId { get; set; }
        [Comment("Unique identifier of the destination area")]
        public Guid DestinationAreaId { get; set; }
        [Comment("Distance between source and destination areas")]
        public decimal Distance { get; set; } 

    }
}

