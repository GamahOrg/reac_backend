using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.IdentityTypeDtos;

public partial class SysIdentityTypeDto : BaseEntity<IdentityType>
{
    public string IdentityType { get; set; } = null!;


}
