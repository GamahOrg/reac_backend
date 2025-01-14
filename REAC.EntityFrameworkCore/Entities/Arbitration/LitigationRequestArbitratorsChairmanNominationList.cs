using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorsChairmanNominationList : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }
        public Guid ArbitratorChairmanCitizenId { get; set; }
        public int? TotalPriority { get; set; }
        public bool IsSent { get; set; }
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorChairmanCitizen { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitratorsChairmanNominationListValue> LitigationRequestArbitratorsChairmanNominationListValues { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationListValue>();

    }

}
