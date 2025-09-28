using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventActionMode : BaseEntity<EventActionMode>
    {
        [Comment("Description for Action Mode")]
        public string ActionMode { get; set; } = null!;

        public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEvents { get; set; } = new List<WorkFlowStepEvent>();

        public virtual ICollection<WorkFlowStepEventActionModeTranslation> Translations { get; set; } = new List<WorkFlowStepEventActionModeTranslation>();
    }
}
