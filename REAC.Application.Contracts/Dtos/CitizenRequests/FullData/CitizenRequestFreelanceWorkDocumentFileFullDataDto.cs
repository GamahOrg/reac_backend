using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestFreelanceWorkDocumentFileFullDataDto : BaseEntity<Guid>
    {
        public Guid FreelanceWorkDocumentFileId { get; set; }
        public SysFileManagerCreateUpdateDto FreelanceWorkDocumentFile { get; set; }

    }

}
