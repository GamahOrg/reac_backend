using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventSecurityLevelActorType : BaseEntity<SecurityLevelActorType>
{
    public string SecurityLevelActorType { get; set; } = null!;

    public virtual ICollection<WorkFlowStepEventSecurityLevelActorTypeTranslation> Translations { get; set; } = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>();

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();
}
