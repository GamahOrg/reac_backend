using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepStage;

public partial class WorkFlowStepStageDto : BaseEntity<Guid>
{
    public int? Order { get; set; }

    public string StageName { get; set; } = null!;

}
