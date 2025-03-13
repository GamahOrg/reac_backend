using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestReconciliationCenterDegreeFileFullDataDto : BaseEntity<Guid>
    {
        public Guid ReconciliationCenterDegreeFileId { get; set; }
        public SysFileManagerCreateUpdateDto ReconciliationCenterFile { get; set; }

    }
}
