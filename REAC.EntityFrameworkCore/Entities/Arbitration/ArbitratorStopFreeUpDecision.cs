using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class ArbitratorStopFreeUpDecision : BaseEntity<int>
    {
        public Guid CitizenId { get; set; }
        public ArbitratorStatus ArbitratorStatusId { get; set; }
        public DateTime DecisionDate { get; set; }
        public DateTime? DecisionFrom { get; set; }
        public DateTime? DecisionTo { get; set; }
        public StoppingReason? StoppingReason { get; set; }
        public string? LateLitigationRequestNumber { get; set; }
        public Citizen Citizen { get; set; } = null!;
        public virtual SysArbitratorStatus? SysArbitratorStatus { get; set; }

    }
}
