using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;

public partial class WorkFlowStepEventCreateUpdateDto : WorkFlowStepEventDto
{

    public List<WorkFlowStepEventTranslation> Translations { get; set; } = new List<WorkFlowStepEventTranslation>();

}
