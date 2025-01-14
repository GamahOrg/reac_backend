using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEventSecurityLevel
{
    public class WorkFlowStepEventSecurityLevelDto : FullAuditedBaseEntity<EventSecurityLevel>
    {
        public string SecurityLevel { get; set; } = null!;

    }
}
