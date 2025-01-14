using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.ReformerStatusDto
{
    public class SysReformerStatusDto : BaseEntity<ReformerStatus>
    {
        public string StatusName { get; set; } = null!;
    }
}
