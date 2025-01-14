using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepStage : FullAuditedBaseEntity<Guid>
{
    public int? Order { get; set; }

    public string StageName { get; set; } = null!;

    public virtual ICollection<WorkFlowStepStageTranslation> Translations { get; set; } = new List<WorkFlowStepStageTranslation>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();
}
