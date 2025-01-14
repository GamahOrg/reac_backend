using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorCount : BaseEntity<Guid>
    {
        public int ArbitratorCount { get; set; }
    }
}
