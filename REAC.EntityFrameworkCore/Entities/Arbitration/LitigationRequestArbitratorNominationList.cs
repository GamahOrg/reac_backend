using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorNominationList : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid ArbitratorCitizenId { get; set; }
        [Comment("Priority ranking assigned by the plaintiff for this arbitrator")]
        public int? PlaintiffPriority { get; set; }
        [Comment("Priority ranking assigned by the defendant for this arbitrator")]
        public int? DefendantPriority { get; set; }
        [Comment("Total combined priority score for this arbitrator")]
        public int? TotalPriority { get; set; }
        [Comment("Indicates whether the nomination list has been sent")]
        public bool IsSent { get; set; }
        [Comment("Indicates whether the nomination has been escalated to management")]
        public bool? IsEscalated { get; set; }
        [Comment("Date when the nomination was escalated")]
        public DateTime? EscalateDate { get; set; }
        [Comment("Indicates whether this arbitrator was selected by the manager")]
        public bool? IsSelectedFromManager { get; set; }
        [Comment("Indicates whether this arbitrator was selected by the plaintiff")]
        public bool? IsPlaintiffSelected { get; set; }
        [Comment("Indicates whether this arbitrator was selected by the defendant")]
        public bool? IsDefendantSelected { get; set; }
        public bool? IsArbitratorAdminSelected { get; set; }
        [Comment("Indicates whether a response has been received for this nomination")]
        public bool? IsResponse { get; set; }
          
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();



    }

}

