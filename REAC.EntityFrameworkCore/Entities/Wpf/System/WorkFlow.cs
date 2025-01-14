using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlow : FullAuditedBaseEntity<Guid>
{
    public string WorkFlowName { get; set; } = null!;

    public Guid? DepartmentId { get; set; }

    public virtual SysDepartment? Department { get; set; }

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();

    public virtual ICollection<WorkFlowTranslation> Translations { get; set; } = new List<WorkFlowTranslation>();

}
