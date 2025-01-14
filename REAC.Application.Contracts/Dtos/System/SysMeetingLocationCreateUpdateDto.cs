using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System
{
    public class SysMeetingLocationCreateUpdateDto : BaseEntity<Guid>
    {
        public string MeetingLocationName { get; set; } = null!;
    }
}
