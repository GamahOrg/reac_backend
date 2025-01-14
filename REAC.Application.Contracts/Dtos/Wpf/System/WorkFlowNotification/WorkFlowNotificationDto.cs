using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotification;

public partial class WorkFlowNotificationDto : BaseEntity<Guid>
{
    public Guid WorkFlowNotificationPlanId { get; set; }

    public Guid WorkFlowNotificationDestinationTypeId { get; set; }

    public Guid WorkFlowNotificationTypeId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? NotificationMessage { get; set; }


}
