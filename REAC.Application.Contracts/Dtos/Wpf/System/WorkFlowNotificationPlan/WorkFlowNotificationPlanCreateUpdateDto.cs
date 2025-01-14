using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationPlan;

public partial class WorkFlowNotificationPlanCreateUpdateDto : WorkFlowNotificationPlanDto
{
    public List<WorkFlowNotificationPlanTranslation> Translations { get; set; } = new List<WorkFlowNotificationPlanTranslation>();
}
