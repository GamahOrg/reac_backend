using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public class CitizenRequestNoteCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid CitizenRequestId { get; set; }
        public string Notes { get; set; } = null!;
    }
}
