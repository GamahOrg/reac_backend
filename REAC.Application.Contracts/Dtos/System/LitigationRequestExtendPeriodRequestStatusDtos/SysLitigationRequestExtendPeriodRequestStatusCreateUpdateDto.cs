using REAC.Application.Contracts.Dtos.System.LitigationRequestExtendPeriodRequestStatusDtos;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestExtendPeriodRequestStatusDtos;

public partial class SysLitigationRequestExtendPeriodRequestStatusCreateUpdateDto : SysLitigationRequestExtendPeriodRequestStatusDto
{
    public virtual ICollection<SysLitigationRequestExtendPeriodRequestStatusTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestExtendPeriodRequestStatusTranslationCreateUpdateDto>();
}
