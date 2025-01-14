using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationType;

public partial class WorkFlowNotificationTypeCreateUpdateDto : WorkFlowNotificationTypeDto
{


    public List<WorkFlowNotificationTypeTranslation> Translations { get; set; } = new List<WorkFlowNotificationTypeTranslation>();

}
