using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepWorkMode : BaseEntity<StepWorkMode>
{
    public string WorkMode { get; set; } = null!;

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();

    public virtual ICollection<WorkFlowStepWorkModeTranslation> Translations { get; set; } = new List<WorkFlowStepWorkModeTranslation>();

}
