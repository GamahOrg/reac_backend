using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitratorRecusalRequestFile : BaseEntity<Guid>
    {
        public Guid LitigationRequestArbitratorRecusalRequestId { get; set; }
        public Guid RecusalRequestFileId { get; set; }
        public virtual LitigationRequestArbitratorRecusalRequest LitigationRequestArbitratorRecusalRequest { get; set; } = null!;
        public virtual SysFileManager RecusalRequestFile { get; set; } = null!;
    }

}
