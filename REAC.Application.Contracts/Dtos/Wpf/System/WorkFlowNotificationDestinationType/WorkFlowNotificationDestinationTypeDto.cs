using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationDestinationType;

public partial class WorkFlowNotificationDestinationTypeDto : BaseEntity<Guid>
{
    public string DestinationType { get; set; } = null!;

}
