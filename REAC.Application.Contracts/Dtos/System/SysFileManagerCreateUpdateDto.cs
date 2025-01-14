using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysFileManagerCreateUpdateDto : BaseEntity<Guid>
{
    public Guid Id { get; set; }
    public string? FileDescription { get; set; }

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

}
