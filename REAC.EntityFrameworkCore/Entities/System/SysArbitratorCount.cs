using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorCount : BaseEntity<Guid>
    {
        [Comment("Number of arbitrators")]
        public int ArbitratorCount { get; set; }
    }
}

