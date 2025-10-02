using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class ArbitratorStopFreeUpDecision : BaseEntity<int>
    {
        [Comment("Unique identifier of the citizen")]
        public Guid CitizenId { get; set; }
        [Comment("Status identifier")]
        public ArbitratorStatus ArbitratorStatusId { get; set; }
        [Comment("Date when the arbitrator stop/free up decision was made")]
        public DateTime DecisionDate { get; set; }
        [Comment("Start date of the arbitrator stop/free up decision period")]
        public DateTime? DecisionFrom { get; set; }
        [Comment("End date of the arbitrator stop/free up decision period")]
        public DateTime? DecisionTo { get; set; }
        [Comment("Reason for stopping or freeing up the arbitrator")]
        public StoppingReason? StoppingReason { get; set; }
        [Comment("Number of the late litigation request")]
        public string? LateLitigationRequestNumber { get; set; }
        [Comment("The citizen who is the arbitrator subject to this decision")]
        public Citizen Citizen { get; set; } = null!;
        public virtual SysArbitratorStatus? SysArbitratorStatus { get; set; }

    }
}


