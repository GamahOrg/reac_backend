using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestAcademicDegreeFile : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }

    public Guid AcademicDegreeFileId { get; set; }

    public virtual SysFileManager? AcademicDegreeFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}


