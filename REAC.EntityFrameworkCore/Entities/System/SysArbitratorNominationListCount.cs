using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorNominationListCount : BaseEntity<Guid>
    {
        public int ArbitratorCount { get; set; }
        public int ArbitratorNominationListCount { get; set; }
        public int ArbitratorChairmanNominationListCount { get; set; }
    }

}
