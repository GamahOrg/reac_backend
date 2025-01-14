using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestProgramRealEstateInstituteFile : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }

    public Guid RealEstateInstituteFileId { get; set; }

    public virtual SysFileManager? ProgramRealEstateDocumentFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}

