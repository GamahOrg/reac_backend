using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

public partial class SysLitigationRequestCategoryDto : BaseEntity<LitigationRequestCategory>
{
    /// <summary>
    /// صلح – تحكيم-صلح وتحكيم
    /// </summary>
    public string CategoryName { get; set; } = null!;

    public string? AcknowledgmentContent { get; set; }

    public string? TermsAndConditions { get; set; }

    public Guid BpfId { get; set; }


}
