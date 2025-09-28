using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationDestinationType : BaseEntity<Guid>
{
    [Comment("Description for Destination Type")]
    public string DestinationType { get; set; } = null!;

    public virtual ICollection<WorkFlowNotificationDestinationTypeTranslation> Translations { get; set; } = new List<WorkFlowNotificationDestinationTypeTranslation>();

    public virtual ICollection<WorkFlowNotification> WorkFlowNotifications { get; set; } = new List<WorkFlowNotification>();
}

