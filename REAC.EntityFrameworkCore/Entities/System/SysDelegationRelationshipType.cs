using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysDelegationRelationshipType : BaseEntity<DelegationRelationshipType>
{
    public string DelegationRelationshipTypeName { get; set; } = null!;

    public virtual ICollection<Delegator> Delegators { get; set; } = new List<Delegator>();
    public virtual ICollection<SysDelegationRelationshipTypeTranslation> Translations { get; set; } = new List<SysDelegationRelationshipTypeTranslation>();

}

