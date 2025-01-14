using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStep : FullAuditedBaseEntity<Guid>
{
    public Guid WorkFlowId { get; set; }

    public string StepName { get; set; } = null!;

    public string CreationLogMessage { get; set; } = null!;

    public bool ShowForApplicant { get; set; }

    public int Order { get; set; }
    public ProcessActorType ProcessActorType { get; set; }

    public StepActorType ActorTypeId { get; set; }

    public StepWorkMode WorkModeId { get; set; }

    public Guid? StageId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? EmployeeId { get; set; }

    public RequestStatus? InternalStatusId { get; set; }

    public RequestStatus? ExternalStatusId { get; set; }

    public Guid? CreationNotificationPlanId { get; set; }

    public virtual WorkFlowNotificationPlan? CreationNotificationPlan { get; set; }

    public virtual ICollection<WorkFlowStepTranslation> Translations { get; set; } = new List<WorkFlowStepTranslation>();

    public virtual WorkFlowStepsActorType ActorType { get; set; } = null!;

    public virtual SysDepartment? Department { get; set; }

    public virtual SysEmployee? Employee { get; set; }

    public virtual WorkFlowStepStage? Stage { get; set; }

    public virtual WorkFlow WorkFlow { get; set; } = null!;

    public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEventNextWorkFlowStepDestinations { get; set; } = new List<WorkFlowStepEvent>();

    public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEvents { get; set; } = new List<WorkFlowStepEvent>();

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

    public virtual WorkFlowStepWorkMode WorkMode { get; set; } = null!;
}
