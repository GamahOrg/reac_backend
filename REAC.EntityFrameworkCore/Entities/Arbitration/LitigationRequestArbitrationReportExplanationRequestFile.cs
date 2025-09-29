using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitrationReportExplanationRequestFile : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the arbitration report explanation request")]
        public Guid ArbitrationReportExplanationRequestId { get; set; }
        [Comment("Unique identifier of the arbitration report explanation request")]
        public Guid ExplanationRequestFileId { get; set; }

        public virtual SysFileManager ExplanationRequestFile { get; set; } = null!;
        public virtual LitigationRequestArbitrationReportExplanationRequest ArbitrationReportExplanationRequest { get; set; } = null!;


    }
}

