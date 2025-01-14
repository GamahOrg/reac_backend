using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestExperienceCertificateFileFullDataDto : BaseEntity<Guid>
    {
        public Guid ExperienceCertificateFileId { get; set; }
        public SysFileManagerCreateUpdateDto ExperienceCertificateFile { get; set; }
    }
}
