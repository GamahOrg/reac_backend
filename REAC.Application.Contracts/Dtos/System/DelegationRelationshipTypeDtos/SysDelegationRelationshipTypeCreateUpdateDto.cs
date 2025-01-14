namespace REAC.Application.Contracts.Dtos.System.GenderDtos;

public partial class SysDelegationRelationshipTypeCreateUpdateDto : SysDelegationRelationshipTypeDto
{
    public virtual ICollection<SysDelegationRelationshipTypeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysDelegationRelationshipTypeTranslationCreateUpdateDto>();
}
