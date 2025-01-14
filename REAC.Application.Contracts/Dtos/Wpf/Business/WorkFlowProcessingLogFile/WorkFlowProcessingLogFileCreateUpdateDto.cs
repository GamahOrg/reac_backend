using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLogFile;

public partial class WorkFlowProcessingLogFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid FileId { get; set; }

}
public partial class WorkFlowProcessingLogFileAllDto : BaseEntity<Guid>
{
    public virtual SysFileManagerCreateUpdateDto? FileManager { get; set; }

}

