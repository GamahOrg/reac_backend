using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestExperienceCertificateFile : BaseEntity<Guid>
{
    public Guid CitizenRequestExperienceCertificateId { get; set; }

    public Guid ExperienceCertificateFileId { get; set; }

    public virtual SysFileManager? ExperienceCertificateFile { get; set; }

    public virtual CitizenRequestExperienceCertificate? CitizenRequestExperienceCertificate { get; set; }
}




