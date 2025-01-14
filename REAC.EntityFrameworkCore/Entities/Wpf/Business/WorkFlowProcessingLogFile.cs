using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowProcessingLogFile : BaseEntity<Guid>
{
    public Guid WorkFlowProcessingLogId { get; set; }

    public Guid FileId { get; set; }

    public virtual SysFileManager? FileManager { get; set; }
    public virtual WorkFlowProcessingLog WorkFlowProcessingLog { get; set; } = null!;
}
