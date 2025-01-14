using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysGender : BaseEntity<Gender>
{
    public string Gender { get; set; } = null!;

    public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();
    public virtual ICollection<SysGenderTranslation> Translations { get; set; } = new List<SysGenderTranslation>();

}

