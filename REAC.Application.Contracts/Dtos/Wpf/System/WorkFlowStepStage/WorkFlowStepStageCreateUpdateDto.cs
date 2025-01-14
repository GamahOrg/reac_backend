using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepStage;

public partial class WorkFlowStepStageCreateUpdateDto : WorkFlowStepStageDto
{


    public List<WorkFlowStepStageTranslation> Translations { get; set; } = new List<WorkFlowStepStageTranslation>();
}
