using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepWorkMode;

public partial class WorkFlowStepWorkModeDto : BaseEntity<StepWorkMode>
{
    public string WorkMode { get; set; } = null!;


}
