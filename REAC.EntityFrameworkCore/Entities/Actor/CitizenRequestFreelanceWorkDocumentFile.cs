using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestFreelanceWorkDocumentFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Unique identifier of the freelance work document file")]
    public Guid FreelanceWorkDocumentFileId { get; set; }

    public virtual SysFileManager? FreelanceWorkDocumentFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}


