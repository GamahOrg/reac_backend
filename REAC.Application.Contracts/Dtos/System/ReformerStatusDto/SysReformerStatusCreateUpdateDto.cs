using REAC.Application.Contracts.Dtos.System.ReformerStatusDto;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysReformerStatusCreateUpdateDto : SysReformerStatusDto
    {
        public virtual ICollection<SysReformerStatusTranslationCreateUpdateDto> Translations { get; set; } = new List<SysReformerStatusTranslationCreateUpdateDto>();
    }
}
