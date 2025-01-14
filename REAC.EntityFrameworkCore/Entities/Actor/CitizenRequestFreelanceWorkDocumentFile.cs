using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestFreelanceWorkDocumentFile : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }

    public Guid FreelanceWorkDocumentFileId { get; set; }

    public virtual SysFileManager? FreelanceWorkDocumentFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}

