using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorAssigningRequestType : BaseEntity<ArbitratorAssigningRequestType>
    {
        public string ArbitratorAssigningRequestName { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();

    }
}
