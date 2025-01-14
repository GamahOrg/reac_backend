using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestExperienceCertificateFileCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid ExperienceCertificateFileId { get; set; }
    }
}
