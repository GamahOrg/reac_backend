using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorsChairmanNominationListValue : BaseEntity<Guid>
    {
        public Guid LitigationRequestArbitratorsChairmanNominationListId { get; set; }
        public Guid ArbitratorChairmanCitizenId { get; set; }
        public Guid ArbitratorCitizenId { get; set; }

        public int Priority { get; set; }
        public bool IsSent { get; set; }

        public virtual LitigationRequestArbitratorsChairmanNominationList LitigationRequestArbitratorsChairmanNominationList { get; set; } = null!;
        public virtual Citizen ArbitratorChairmanCitizen { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;

    }
}
