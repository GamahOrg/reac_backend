using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;

public partial class SysLitigationRequestOwnerTypeDto : BaseEntity<LitigationRequestOwnerType>
{
    /// <summary>
    /// مقدم عن نفسه – مفوض
    /// </summary>
    public string LitigationRequestOwnerType { get; set; } = null!;


}
