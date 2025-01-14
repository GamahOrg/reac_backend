using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventActionMode : BaseEntity<EventActionMode>
    {
        public string ActionMode { get; set; } = null!;

        public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEvents { get; set; } = new List<WorkFlowStepEvent>();

        public virtual ICollection<WorkFlowStepEventActionModeTranslation> Translations { get; set; } = new List<WorkFlowStepEventActionModeTranslation>();
    }
}
