using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysDelegationRelationshipType : BaseEntity<DelegationRelationshipType>
{
    [Comment("Type of the name")]
    public string DelegationRelationshipTypeName { get; set; } = null!;

    public virtual ICollection<Delegator> Delegators { get; set; } = new List<Delegator>();
    public virtual ICollection<SysDelegationRelationshipTypeTranslation> Translations { get; set; } = new List<SysDelegationRelationshipTypeTranslation>();

}

