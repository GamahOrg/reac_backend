using Core.GenericRepository.Shared.BaseEntities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysSetting : BaseEntity<Guid>
{
    [Comment("Configuration setting key identifier")]
    public string Key { get; set; } = null!;
    [Comment("Configuration setting value")]
    public string Value { get; set; } = null!;
}
