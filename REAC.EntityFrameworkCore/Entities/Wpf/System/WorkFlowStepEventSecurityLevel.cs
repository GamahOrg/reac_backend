using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventSecurityLevel : BaseEntity<EventSecurityLevel>
    {
        [Comment("Description for Security Level")]
        public string SecurityLevel { get; set; } = null!;

        public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEvents { get; set; } = new List<WorkFlowStepEvent>();

        public virtual ICollection<WorkFlowStepEventSecurityLevelTranslation> Translations { get; set; } = new List<WorkFlowStepEventSecurityLevelTranslation>();
    }
}
