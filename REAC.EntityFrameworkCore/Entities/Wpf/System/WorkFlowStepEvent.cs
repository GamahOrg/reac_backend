using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEvent : FullAuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the work flow steps")]
    public Guid WorkFlowStepsId { get; set; }
    [Comment("Description for Custom Action Type")]
    public CustomActionType? CustomActionType { get; set; }
    [Comment("Name of the event")]
    public string EventName { get; set; } = null!;

    [Comment("Description for Litigation Request Stage")]
    public LitigationRequestStage? LitigationRequestStage { get; set; }
    [Comment("Description for Litigation Request Sub Stage")]
    public LitigationRequestSubStage? LitigationRequestSubStage { get; set; }

    #region User Config
    [Comment("File")]
    public bool AllowFileUpload { get; set; }

    [Comment("Indicates whether notes are allowed for this workflow step event")]
    public bool AllowNotes { get; set; }
    #endregion

    #region Log Config
    [Comment("Description for Show Processing Log For Applicant")]
    public bool ShowProcessingLogForApplicant { get; set; }

    [Comment("Description for Show Log For Applicant")]
    public bool ShowLogForApplicant { get; set; }

    [Comment("Description for Log Message")]
    public string? LogMessage { get; set; }
    #endregion

    [Comment("Unique identifier of the event security level")]
    public EventSecurityLevel EventSecurityLevelId { get; set; }

    [Comment("Description for Is Admin Approval Required")]
    public bool IsAdminApprovalRequired { get; set; }

    #region InternalStatus
    [Comment("Description for Allow Change Internal Status")]
    public bool AllowChangeInternalStatus { get; set; }

    [Comment("Status identifier")]
    public RequestStatus? InternalStatusId { get; set; }
    #endregion

    #region ExternalStatus
    [Comment("Description for Allow Change External Status")]
    public bool AllowChangeExternalStatus { get; set; }

    [Comment("Status identifier")]
    public RequestStatus? ExternalStatusId { get; set; }
    #endregion

    [Comment("Description for Same Employee")]
    public bool SameEmployee { get; set; }

    [Comment("Unique identifier of the event action mode")]
    public EventActionMode? ActionModeId { get; set; }

    [Comment("Unique identifier of the next work flow step destination")]
    public Guid? NextWorkFlowStepDestinationId { get; set; }



    #region CompletionNotificationPlan
    [Comment("Description for Allow Completion Notification Plan")]
    public bool AllowCompletionNotificationPlan { get; set; }

    [Comment("Unique identifier of the completion notification plan")]
    public Guid? CompletionNotificationPlanId { get; set; }
    #endregion

    #region ActiveValue
    [Comment("Description for Allow Change Is Allow Edit")]
    public bool AllowChangeIsAllowEdit { get; set; }

    [Comment("Value")]
    public bool? IsAllowEditValue { get; set; }
    #endregion

    #region ActiveValue
    [Comment("Indicates whether the record is active")]
    public bool AllowChangeIsActive { get; set; }

    [Comment("Value")]
    public bool? IsActiveValue { get; set; }
    #endregion

    #region FinishedValue
    [Comment("Description for Allow Change Is Finished")]
    public bool AllowChangeIsFinished { get; set; }

    [Comment("Value")]
    public bool? IsFinishedValue { get; set; }
    #endregion

    [Comment("Description for Work Flow Step Event Security Level")]
    public WorkFlowStepEventSecurityLevel WorkFlowStepEventSecurityLevel { get; set; } = null;

    [Comment("Description for Work Flow Step Event Action Mode")]
    public WorkFlowStepEventActionMode? WorkFlowStepEventActionMode { get; set; }

    public virtual WorkFlowNotificationPlan? CompletionNotificationPlan { get; set; }

    public virtual WorkFlowStep? NextWorkFlowStepDestination { get; set; }

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

    public virtual WorkFlowStep WorkFlowSteps { get; set; } = null!;

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();

    public virtual ICollection<WorkFlowStepEventTranslation> Translations { get; set; } = new List<WorkFlowStepEventTranslation>();
}




