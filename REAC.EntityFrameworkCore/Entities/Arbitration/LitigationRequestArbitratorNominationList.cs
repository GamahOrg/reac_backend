using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorNominationList : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }
        public Guid ArbitratorCitizenId { get; set; }
        public int? PlaintiffPriority { get; set; }
        public int? DefendantPriority { get; set; }
        public int? TotalPriority { get; set; }
        public bool IsSent { get; set; }
        public bool? IsEscalated { get; set; }
        public DateTime? EscalateDate { get; set; }
        public bool? IsSelectedFromManager { get; set; }
        public bool? IsPlaintiffSelected { get; set; }
        public bool? IsDefendantSelected { get; set; }
        public bool? IsResponse { get; set; }
          
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();



    }

}
