using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepStage : FullAuditedBaseEntity<Guid>
{
    [Comment("Display order of the workflow step stage")]
    public int? Order { get; set; }

    [Comment("Name of the stage")]
    public string StageName { get; set; } = null!;

    public virtual ICollection<WorkFlowStepStageTranslation> Translations { get; set; } = new List<WorkFlowStepStageTranslation>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();
}

