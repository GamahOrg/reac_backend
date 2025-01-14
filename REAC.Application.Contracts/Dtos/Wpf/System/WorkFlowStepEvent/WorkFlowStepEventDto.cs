using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;

public partial class WorkFlowStepEventDto : BaseEntity<Guid>
{
    public Guid WorkFlowStepsId { get; set; }

    public string EventName { get; set; } = null!;

    public bool AllowFileUpload { get; set; }

    public bool AllowNotes { get; set; }

    public bool AllowChangeInternalStatus { get; set; }

    public bool IsAdminApprovalRequired { get; set; }

    public int? InternalStatusId { get; set; }

    public bool AllowChangeExternalStatus { get; set; }

    public int? ExternalStatusId { get; set; }

    public EventActionMode? ActionModeId { get; set; }

    public Guid? NextWorkFlowStepDestinationId { get; set; }

    public string? LogMessage { get; set; }

    public bool ShowProcessingLogForApplicant { get; set; }

    public bool ShowLogForApplicant { get; set; }

    public bool AllowCompletionNotificationPlan { get; set; }

    public Guid? CompletionNotificationPlanId { get; set; }
    public bool SameEmployee { get; set; }

    public bool ChangeIsActive { get; set; }

    public bool? IsActiveValue { get; set; }

    public bool ChangeIsFinished { get; set; }

    public bool? IsFinishedValue { get; set; }

}
