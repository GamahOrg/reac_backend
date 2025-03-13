using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestProgramRealEstateInstituteFileFullDataDto : BaseEntity<Guid>
    {
        public Guid RealEstateInstituteFileId { get; set; }
        public SysFileManagerCreateUpdateDto? ProgramRealEstateDocumentFile { get; set; } = null;

    }

}
