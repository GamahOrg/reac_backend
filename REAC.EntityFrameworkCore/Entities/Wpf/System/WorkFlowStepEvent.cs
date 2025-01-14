using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEvent : FullAuditedBaseEntity<Guid>
{
    public Guid WorkFlowStepsId { get; set; }
    public CustomActionType? CustomActionType { get; set; }
    public string EventName { get; set; } = null!;

    public LitigationRequestStage? LitigationRequestStage { get; set; }
    public LitigationRequestSubStage? LitigationRequestSubStage { get; set; }

    #region User Config
    public bool AllowFileUpload { get; set; }

    public bool AllowNotes { get; set; }
    #endregion

    #region Log Config
    public bool ShowProcessingLogForApplicant { get; set; }

    public bool ShowLogForApplicant { get; set; }

    public string? LogMessage { get; set; }
    #endregion

    public EventSecurityLevel EventSecurityLevelId { get; set; }

    public bool IsAdminApprovalRequired { get; set; }

    #region InternalStatus
    public bool AllowChangeInternalStatus { get; set; }

    public RequestStatus? InternalStatusId { get; set; }
    #endregion

    #region ExternalStatus
    public bool AllowChangeExternalStatus { get; set; }

    public RequestStatus? ExternalStatusId { get; set; }
    #endregion

    public bool SameEmployee { get; set; }

    public EventActionMode? ActionModeId { get; set; }

    public Guid? NextWorkFlowStepDestinationId { get; set; }



    #region CompletionNotificationPlan
    public bool AllowCompletionNotificationPlan { get; set; }

    public Guid? CompletionNotificationPlanId { get; set; }
    #endregion

    #region ActiveValue
    public bool AllowChangeIsAllowEdit { get; set; }

    public bool? IsAllowEditValue { get; set; }
    #endregion

    #region ActiveValue
    public bool AllowChangeIsActive { get; set; }

    public bool? IsActiveValue { get; set; }
    #endregion

    #region FinishedValue
    public bool AllowChangeIsFinished { get; set; }

    public bool? IsFinishedValue { get; set; }
    #endregion

    public WorkFlowStepEventSecurityLevel WorkFlowStepEventSecurityLevel { get; set; } = null;

    public WorkFlowStepEventActionMode? WorkFlowStepEventActionMode { get; set; }

    public virtual WorkFlowNotificationPlan? CompletionNotificationPlan { get; set; }

    public virtual WorkFlowStep? NextWorkFlowStepDestination { get; set; }

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

    public virtual WorkFlowStep WorkFlowSteps { get; set; } = null!;

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();

    public virtual ICollection<WorkFlowStepEventTranslation> Translations { get; set; } = new List<WorkFlowStepEventTranslation>();
}


