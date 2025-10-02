using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestProgramRealEstateInstituteFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Unique identifier of the real estate institute program file")]
    public Guid RealEstateInstituteFileId { get; set; }

    public virtual SysFileManager? ProgramRealEstateDocumentFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}


