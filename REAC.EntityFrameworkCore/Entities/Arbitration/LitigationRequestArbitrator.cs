using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration;

public partial class LitigationRequestArbitratorList : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Unique identifier of the litigation request")]
    public Guid ArbitratorCitizenId { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    public virtual Citizen ArbitratorCitizen { get; set; } = null!;

}


