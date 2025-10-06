using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStep : FullAuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the workflow")]
    public Guid WorkFlowId { get; set; }

    [Comment("Name of the step")]
    public string StepName { get; set; } = null!;

    [Comment("Message logged when this step is created")]
    public string CreationLogMessage { get; set; } = null!;

    [Comment("Indicates whether this step is visible to the applicant")]
    public bool ShowForApplicant { get; set; }

    [Comment("Order of this step in the workflow sequence")]
    public int Order { get; set; }
    [Comment("Type of actor responsible for processing this step")]
    public ProcessActorType ProcessActorType { get; set; }

    [Comment("Unique identifier of the step actor type")]
    public StepActorType ActorTypeId { get; set; }

    [Comment("Unique identifier of the step work mode")]
    public StepWorkMode WorkModeId { get; set; }

    [Comment("Unique identifier of the stage")]
    public Guid? StageId { get; set; }

    [Comment("Unique identifier of the department")]
    public Guid? DepartmentId { get; set; }

    [Comment("Unique identifier of the employee")]
    public Guid? EmployeeId { get; set; }

    [Comment("Internal status of the request at this step")]
    public RequestStatus? InternalStatusId { get; set; }

    [Comment("External status of the request at this step")]
    public RequestStatus? ExternalStatusId { get; set; }

    [Comment("Unique identifier of the creation notification plan")]
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


