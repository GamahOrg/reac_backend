using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestCVFileFullDataDto : BaseEntity<Guid>
    {
        public Guid CVFileId { get; set; }
        public SysFileManagerCreateUpdateDto? CvFile { get; set; }

    }
}
