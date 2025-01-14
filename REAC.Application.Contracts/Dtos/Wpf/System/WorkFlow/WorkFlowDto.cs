using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlow;

public partial class WorkFlowDto : BaseEntity<Guid>
{
    public string WorkFlowName { get; set; } = null!;


}
