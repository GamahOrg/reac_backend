using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationDestinationType;

public partial class WorkFlowNotificationDestinationTypeCreateUpdateDto : WorkFlowNotificationDestinationTypeDto
{
    public List<WorkFlowNotificationDestinationTypeTranslation> Translations { get; set; } = new List<WorkFlowNotificationDestinationTypeTranslation>();

}
