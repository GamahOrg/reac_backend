using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Litigation.Rating;

public class BeneficiarySatisfactionSurveyDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }
    public Guid CitizenId { get; set; }
    public int StageType { get; set; }
    public int? ProcedureSatisfaction { get; set; }
    public int? PerformanceSatisfaction { get; set; }
    public bool IsRated { get; set; }
    public DateTime CreatedOn { get; set; }
}
