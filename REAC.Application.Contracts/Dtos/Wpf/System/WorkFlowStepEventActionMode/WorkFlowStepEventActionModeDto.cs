using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEventActionMode
{
    public class WorkFlowStepEventActionModeDto : FullAuditedBaseEntity<EventActionMode>
    {
        public string ActionMode { get; set; } = null!;

    }
}
