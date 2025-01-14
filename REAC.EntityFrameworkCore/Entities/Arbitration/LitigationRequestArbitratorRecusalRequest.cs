using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitratorRecusalRequest : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public Guid ArbitratorCitizenId { get; set; }

        public string Reason { get; set; } = null!;

        public DateTime RequestOn { get; set; }
        public LitigationRequestParty RequestParty { get; set; }

        public ApproveRejectStatus? ApproveRejectStatus { get; set; }

        public DateTime? ResponseOn { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;

        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorRecusalRequestFile> LitigationRequestArbitratorRecusalRequestFiles { get; set; } = new List<LitigationRequestArbitratorRecusalRequestFile>();

    }



}
