using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepsActorType : BaseEntity<StepActorType>
{
    [Comment("Description for Actor Type")]
    public string ActorType { get; set; } = null!;

    public virtual ICollection<WorkFlowStepsActorTypeTranslation> Translations { get; set; } = new List<WorkFlowStepsActorTypeTranslation>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();
}

