using Core.GenericRepository.Shared.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysSetting : BaseEntity<Guid>
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
