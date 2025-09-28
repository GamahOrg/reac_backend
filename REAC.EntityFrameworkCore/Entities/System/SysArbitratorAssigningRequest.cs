using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorAssigningRequestType : BaseEntity<ArbitratorAssigningRequestType>
    {
        [Comment("Name of the arbitrator assigning request")]
        public string ArbitratorAssigningRequestName { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();

    }
}

