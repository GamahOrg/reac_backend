using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.AreaDto
{
    public class SysCitizenRequestTypeDto : BaseEntity<Guid>
    {
        public string CitizenRequestTypeName { get; set; } = null!;
    }
}
