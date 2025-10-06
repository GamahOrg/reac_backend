using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventSecurityLevelActorType : BaseEntity<SecurityLevelActorType>
{
    [Comment("Description for Security Level Actor Type")]
    public string SecurityLevelActorType { get; set; } = null!;

    public virtual ICollection<WorkFlowStepEventSecurityLevelActorTypeTranslation> Translations { get; set; } = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>();

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();
}

