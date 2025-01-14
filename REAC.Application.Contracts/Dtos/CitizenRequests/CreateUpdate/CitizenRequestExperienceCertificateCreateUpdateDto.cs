using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestExperienceCertificateCreateUpdateDto : BaseEntity<Guid>
    {
        public string CertificateName { get; set; } = null!;
        public DateTime AcquisitionDate { get; set; }
        public string IssuingEntityName { get; set; } = null!;
        public virtual ICollection<CitizenRequestExperienceCertificateFileCreateUpdateDto> CitizenRequestExperienceCertificateFiles { get; set; } = new List<CitizenRequestExperienceCertificateFileCreateUpdateDto>();
    }

}
