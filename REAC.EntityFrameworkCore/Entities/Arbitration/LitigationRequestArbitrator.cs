using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Arbitration;

public partial class LitigationRequestArbitratorList : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public Guid ArbitratorCitizenId { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    public virtual Citizen ArbitratorCitizen { get; set; } = null!;

}

