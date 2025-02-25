using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Notification;


public class SysNotificationEmail : BaseEntity<Guid>
{
    public string Body { get; set; }
    public string Subject { get; set; }
    public DateTime CreatedOn { get; set; }
    public int RetryAttempt { get; set; }
    public string ToAddressList { get; set; }
    public bool IsSent { get; set; }

    public string? Error {  get; set; }

}
