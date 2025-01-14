using REAC.Application.Contracts.Dtos.System.ArbitratorStatusDto;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorStatusCreateUpdateDto : SysArbitratorStatusDto
    {
        public virtual ICollection<SysArbitratorStatusTranslationCreateUpdateDto> Translations { get; set; } = new List<SysArbitratorStatusTranslationCreateUpdateDto>();
    }
}
