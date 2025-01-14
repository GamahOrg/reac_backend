using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.GenderDtos;

public partial class SysGenderDto : BaseEntity<Gender>
{
    public string Gender { get; set; } = null!;


}

