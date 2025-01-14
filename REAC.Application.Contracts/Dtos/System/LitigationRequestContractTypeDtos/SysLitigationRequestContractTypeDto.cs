using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;

public partial class SysLitigationRequestContractTypeDto : BaseEntity<LitigationRequestContractType>
{
    public string TypeName { get; set; } = null!;

    public string? AcknowledgmentContent { get; set; }

    public string? TermsAndConditions { get; set; }
}
