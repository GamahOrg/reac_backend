using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public class SysMobileOTP : BaseEntity<Guid>
{
    [Comment("Mobile phone number")]
    public string Mobile { get; set; }
    [Comment("Description for Code")]
    public string Code { get; set; }
    [Comment("Expiry date and time of the OTP")]
    public DateTime ExpiryDateTime { get; set; }
}

