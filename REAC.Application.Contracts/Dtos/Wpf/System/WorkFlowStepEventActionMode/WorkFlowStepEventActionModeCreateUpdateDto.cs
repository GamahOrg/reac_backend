using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEventActionMode
{
    public class WorkFlowStepEventActionModeCreateUpdateDto : WorkFlowStepEventActionModeDto
    {
        public virtual ICollection<WorkFlowStepEventActionModeTranslation> Translations { get; set; } = new List<WorkFlowStepEventActionModeTranslation>();
    }
}
