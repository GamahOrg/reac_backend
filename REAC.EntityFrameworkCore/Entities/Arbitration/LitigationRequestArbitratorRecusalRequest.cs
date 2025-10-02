using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitratorRecusalRequest : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }

        [Comment("Unique identifier of the litigation request")]
        public Guid ArbitratorCitizenId { get; set; }

        [Comment("Description for Reason")]
        public string Reason { get; set; } = null!;

        [Comment("Date when the arbitrator recusal request was submitted")]
        public DateTime RequestOn { get; set; }
        [Comment("Description for Request Party")]
        public LitigationRequestParty RequestParty { get; set; }

        [Comment("Description for Approve Reject Status")]
        public ApproveRejectStatus? ApproveRejectStatus { get; set; }

        [Comment("Date when the arbitrator recusal request was responded to")]
        public DateTime? ResponseOn { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;

        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorRecusalRequestFile> LitigationRequestArbitratorRecusalRequestFiles { get; set; } = new List<LitigationRequestArbitratorRecusalRequestFile>();

    }



}

