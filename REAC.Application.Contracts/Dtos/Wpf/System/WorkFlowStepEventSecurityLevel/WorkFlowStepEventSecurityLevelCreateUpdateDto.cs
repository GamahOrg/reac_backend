using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEventSecurityLevel
{
    public class WorkFlowStepEventSecurityLevelCreateUpdateDto : WorkFlowStepEventSecurityLevelDto
    {
        public virtual ICollection<WorkFlowStepEventSecurityLevelTranslation> Translations { get; set; } = new List<WorkFlowStepEventSecurityLevelTranslation>();
    }
}
