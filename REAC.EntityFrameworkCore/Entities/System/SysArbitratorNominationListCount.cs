using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorNominationListCount : BaseEntity<Guid>
    {
        [Comment("Number of arbitrators")]
        public int ArbitratorCount { get; set; }
        [Comment("Number of arbitrator nomination lists")]
        public int ArbitratorNominationListCount { get; set; }
        [Comment("Number of arbitrator chairman nomination lists")]
        public int ArbitratorChairmanNominationListCount { get; set; }
    }

}

