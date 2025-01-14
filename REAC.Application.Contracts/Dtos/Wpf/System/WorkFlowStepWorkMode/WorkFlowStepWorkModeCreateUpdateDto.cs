using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepWorkMode;

public partial class WorkFlowStepWorkModeCreateUpdateDto : WorkFlowStepWorkModeDto
{
    public List<WorkFlowStepWorkModeTranslation> Translations { get; set; } = new List<WorkFlowStepWorkModeTranslation>();

}
