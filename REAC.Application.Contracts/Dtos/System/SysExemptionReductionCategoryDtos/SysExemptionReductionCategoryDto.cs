using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

public partial class SysExemptionReductionCategoryDto : BaseEntity<Guid>
{
    public string ExemptionReductionCategoryName { get; set; } = null!;

}
