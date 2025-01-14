using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepsActorType;

public partial class WorkFlowStepsActorTypeCreateUpdateDto : WorkFlowStepsActorTypeDto
{

    public List<WorkFlowStepsActorTypeTranslation> Translations { get; set; } = new List<WorkFlowStepsActorTypeTranslation>();
}
