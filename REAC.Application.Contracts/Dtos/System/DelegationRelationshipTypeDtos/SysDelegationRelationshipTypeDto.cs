using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.GenderDtos;

public partial class SysDelegationRelationshipTypeDto : BaseEntity<DelegationRelationshipType>
{
    public string DelegationRelationshipTypeName { get; set; } = null!;


}

