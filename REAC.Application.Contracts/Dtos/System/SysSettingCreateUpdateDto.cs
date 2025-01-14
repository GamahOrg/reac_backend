using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System;

public partial class SysSettingCreateUpdateDto : BaseEntity<Guid>
{
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;
}
