using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration;

public class LitigationRequestArbitrationCorrectionRequestFile : BaseEntity<Guid>
{
    [Comment("Foreign key to the correction request")]
    public Guid LitigationRequestArbitrationCorrectionRequestId { get; set; }

    [Comment("Foreign key to the file stored in SysFileManager")]
    public Guid CorrectionRequestFileId { get; set; }
    public DateTime CreatedOn { get; set; }
    public virtual LitigationRequestArbitrationCorrectionRequest LitigationRequestArbitrationCorrectionRequest { get; set; } = null!;
    public virtual SysFileManager CorrectionRequestFile { get; set; } = null!;
}
