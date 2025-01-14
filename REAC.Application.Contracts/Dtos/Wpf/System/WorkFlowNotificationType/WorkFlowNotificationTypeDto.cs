using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationType;

public partial class WorkFlowNotificationTypeDto : BaseEntity<Guid>
{

    public string NotificationType { get; set; } = null!;


}
