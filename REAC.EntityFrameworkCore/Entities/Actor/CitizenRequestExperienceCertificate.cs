using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestExperienceCertificate : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Name of the experience certificate")]
    public string CertificateName { get; set; } = null!;
    [Comment("Date when the certificate was acquired")]
    public DateTime AcquisitionDate { get; set; }
    [Comment("Name of the issuing entity")]
    public string IssuingEntityName { get; set; } = null!;
    public virtual CitizenRequest? CitizenRequest { get; set; }
    public virtual ICollection<CitizenRequestExperienceCertificateFile>? CitizenRequestExperienceCertificateFiles { get; set; } = new List<CitizenRequestExperienceCertificateFile>();


}

