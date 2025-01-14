using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventSecurityLevel : BaseEntity<EventSecurityLevel>
    {
        public string SecurityLevel { get; set; } = null!;

        public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEvents { get; set; } = new List<WorkFlowStepEvent>();

        public virtual ICollection<WorkFlowStepEventSecurityLevelTranslation> Translations { get; set; } = new List<WorkFlowStepEventSecurityLevelTranslation>();
    }
}
