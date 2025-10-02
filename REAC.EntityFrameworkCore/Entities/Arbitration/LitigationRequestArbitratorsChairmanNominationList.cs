using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorsChairmanNominationList : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid ArbitratorChairmanCitizenId { get; set; }
        [Comment("Total")]
        public int? TotalPriority { get; set; }
        [Comment("Description for Is Sent")]
        public bool IsSent { get; set; }
        [Comment("Description for Response")]
        public LitigationRequestArbitratorAssigningRequestStatus? Response { get; set; }
        [Comment("Description for Is Escalated")]
        public bool? IsEscalated { get; set; }
        [Comment("Date when the chairman nomination was escalated")]
        public DateTime? EscalateDate { get; set; }
        [Comment("Description for Is Selected From Manager")]
        public bool? IsSelectedFromManager { get; set; }
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorChairmanCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorsChairmanNominationListValue> LitigationRequestArbitratorsChairmanNominationListValues { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationListValue>();

    }

}

