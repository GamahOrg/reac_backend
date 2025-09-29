using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestAcademicDegreeFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Unique identifier of the academic degree file")]
    public Guid AcademicDegreeFileId { get; set; }

    public virtual SysFileManager? AcademicDegreeFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}



