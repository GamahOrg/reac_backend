using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotification;

public partial class WorkFlowNotificationCreateUpdateDto : WorkFlowNotificationDto
{

    public List<WorkFlowNotificationTranslation> Translations { get; set; } = new List<WorkFlowNotificationTranslation>();

}
