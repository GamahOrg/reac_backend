using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStep;

public partial class WorkFlowStepCreateUpdateDto : WorkFlowStepDto
{


    public List<WorkFlowStepTranslation> Translations { get; set; } = new List<WorkFlowStepTranslation>();

}
