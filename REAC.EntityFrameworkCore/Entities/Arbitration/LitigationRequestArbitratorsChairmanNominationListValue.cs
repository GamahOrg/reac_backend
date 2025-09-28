using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorsChairmanNominationListValue : BaseEntity<Guid>
    {
        public Guid LitigationRequestArbitratorsChairmanNominationListId { get; set; }
        [Comment("Unique identifier of the arbitrator chairman citizen")]
        public Guid ArbitratorChairmanCitizenId { get; set; }
        [Comment("Unique identifier of the arbitrator citizen")]
        public Guid ArbitratorCitizenId { get; set; }

        [Comment("Description for Priority")]
        public int Priority { get; set; }
        [Comment("Description for Is Sent")]
        public bool IsSent { get; set; }

        public virtual LitigationRequestArbitratorsChairmanNominationList LitigationRequestArbitratorsChairmanNominationList { get; set; } = null!;
        public virtual Citizen ArbitratorChairmanCitizen { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;

    }
}

