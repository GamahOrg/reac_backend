using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepsActorType : BaseEntity<StepActorType>
{
    public string ActorType { get; set; } = null!;

    public virtual ICollection<WorkFlowStepsActorTypeTranslation> Translations { get; set; } = new List<WorkFlowStepsActorTypeTranslation>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();
}
