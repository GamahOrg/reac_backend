using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestExperienceCertificateFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request experience certificate")]
    public Guid CitizenRequestExperienceCertificateId { get; set; }

    [Comment("Unique identifier of the experience certificate file")]
    public Guid ExperienceCertificateFileId { get; set; }

    public virtual SysFileManager? ExperienceCertificateFile { get; set; }

    public virtual CitizenRequestExperienceCertificate? CitizenRequestExperienceCertificate { get; set; }
}




