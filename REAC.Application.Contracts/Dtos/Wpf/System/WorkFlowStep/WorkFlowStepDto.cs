using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStep;

public partial class WorkFlowStepDto : BaseEntity<Guid>
{
    public Guid WorkFlowId { get; set; }

    public string StepName { get; set; } = null!;

    public string CreationLogMessage { get; set; } = null!;

    public bool ShowForApplicant { get; set; }

    public int Order { get; set; }

    public StepActorType ActorTypeId { get; set; }

    public StepWorkMode WorkModeId { get; set; }

    public Guid? StageId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? EmployeeId { get; set; }

    public RequestStatus InternalStatusId { get; set; }

    public RequestStatus ExternalStatusId { get; set; }

    public Guid? CreationNotificationPlanId { get; set; }

}
