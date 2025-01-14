using REAC.Application.Contracts.Dtos.Citizen;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestNotesDto
    {
        public Guid Id { get; set; }
        public Guid LitigationRequestId { get; set; }

        public string? Title { get; set; }

        public string Notes { get; set; } = null!;
        public ICollection<SysFileManagerCreateUpdateDto> LitigationRequestNoteFiles { get; set; } = null!;

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public CitizenShortDto Citizen { get; set; }
    }
}
