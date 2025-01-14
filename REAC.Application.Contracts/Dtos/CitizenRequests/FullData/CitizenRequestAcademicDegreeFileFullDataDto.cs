using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestAcademicDegreeFileFullDataDto : BaseEntity<Guid>
    {
        public Guid AcademicDegreeFileId { get; set; }
        public SysFileManagerCreateUpdateDto AcademicDegreeFile { get; set; }

    }
}
