using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationPlan;

public partial class WorkFlowNotificationPlanDto : BaseEntity<Guid>
{
    public string? NotificationPlan { get; set; }
}