using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestFreelanceWorkDocumentFileCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid FreelanceWorkDocumentFileId { get; set; }

    }

}
