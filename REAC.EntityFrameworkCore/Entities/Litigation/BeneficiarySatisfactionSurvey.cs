using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public class BeneficiarySatisfactionSurvey : AuditedBaseEntity<Guid>
{
    [Comment("FK to LitigationRequest")]
    public Guid LitigationRequestId { get; set; }

    [Comment("FK to Citizen who submitted the survey")]
    public Guid CitizenId { get; set; }

    [Comment("1 = Arbitration, 2 = Reconciliation")]
    public int StageType { get; set; }

    [Comment("Q1: Satisfaction with procedures (1-5), null if dismissed")]
    public int? ProcedureSatisfaction { get; set; }

    [Comment("Q2: Satisfaction with arbitrator/reformer performance (1-5), null if dismissed")]
    public int? PerformanceSatisfaction { get; set; }

    [Comment("True if user submitted ratings, false if dismissed without rating")]
    public bool IsRated { get; set; }

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    public virtual Citizen Citizen { get; set; } = null!;
}
