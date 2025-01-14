using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlow;

public partial class WorkFlowCreateUpdateDto : WorkFlowDto
{

    public List<WorkFlowTranslation> Translations { get; set; } = new List<WorkFlowTranslation>();

}
