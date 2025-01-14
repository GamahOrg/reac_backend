using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitrationReportExplanationRequestFile : BaseEntity<Guid>
    {
        public Guid ArbitrationReportExplanationRequestId { get; set; }
        public Guid ExplanationRequestFileId { get; set; }

        public virtual SysFileManager ExplanationRequestFile { get; set; } = null!;
        public virtual LitigationRequestArbitrationReportExplanationRequest ArbitrationReportExplanationRequest { get; set; } = null!;


    }
}
