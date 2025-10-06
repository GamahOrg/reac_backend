using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities
{
    public class ReformerStopFreeUpDecision : BaseEntity<int>
    {
        [Comment("Unique identifier of the citizen")]
        public Guid CitizenId { get; set; }
        [Comment("Status identifier")]
        public ReformerStatus ReformerStatusId { get; set; }
        [Comment("Date of the reformer decision")]
        public DateTime DecisionDate { get; set; }
        [Comment("Start date of the decision period")]
        public DateTime? DecisionFrom { get; set; }
        [Comment("End date of the decision period")]
        public DateTime? DecisionTo { get; set; }
        [Comment("Reason for stopping the reformer")]
        public StoppingReason? StoppingReason { get; set; }
        [Comment("Number of the late litigation request")]
        public string? LateLitigationRequestNumber { get; set; }
        public Citizen Citizen { get; set; } = null!;
        [Comment("Unique identifier of the user who created this record")]
        public Guid? CreatedBy { get; set; }

        [Comment("Role of the user who created this record")]
        public Seed.Roles? CreatedByRole { get; set; } = null!;
        public virtual SysReformerStatus? SysReformerStatus { get; set; }

    }


}


