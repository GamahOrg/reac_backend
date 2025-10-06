using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitratorRecusalRequestFile : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request arbitrator recusal request")]
        public Guid LitigationRequestArbitratorRecusalRequestId { get; set; }
        [Comment("Unique identifier of the litigation request arbitrator recusal request")]
        public Guid RecusalRequestFileId { get; set; }
        public virtual LitigationRequestArbitratorRecusalRequest LitigationRequestArbitratorRecusalRequest { get; set; } = null!;
        public virtual SysFileManager RecusalRequestFile { get; set; } = null!;
    }

}

