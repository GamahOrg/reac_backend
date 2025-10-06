using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysIdentityType : BaseEntity<IdentityType>
{
    [Comment("Description for Identity Type")]
    public string IdentityType { get; set; } = null!;

    public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();


    public virtual ICollection<SysIdentityTypeTranslation> Translations { get; set; } = new List<SysIdentityTypeTranslation>();

}

