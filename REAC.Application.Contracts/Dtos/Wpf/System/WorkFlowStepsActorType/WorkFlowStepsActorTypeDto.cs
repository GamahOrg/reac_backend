using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepsActorType;

public partial class WorkFlowStepsActorTypeDto : BaseEntity<StepActorType>
{
    public string ActorType { get; set; } = null!;

}
