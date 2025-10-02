using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowProcessingLogFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the work flow processing log")]
    public Guid WorkFlowProcessingLogId { get; set; }

    [Comment("Unique identifier of the work flow processing log")]
    public Guid FileId { get; set; }

    public virtual SysFileManager? FileManager { get; set; }
    public virtual WorkFlowProcessingLog WorkFlowProcessingLog { get; set; } = null!;
}

