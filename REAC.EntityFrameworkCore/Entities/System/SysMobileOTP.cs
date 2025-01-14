using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System;

public class SysMobileOTP : BaseEntity<Guid>
{
    public string Mobile { get; set; }
    public string Code { get; set; }
    public DateTime ExpiryDateTime { get; set; }
}
