using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification;


public class SysNotificationEmail : BaseEntity<Guid>
{
    [Comment("Description for Body")]
    public string Body { get; set; }
    [Comment("Subject")]
    public string Subject { get; set; }
    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }
    [Comment("Description for Retry Attempt")]
    public int RetryAttempt { get; set; }
    public string ToAddressList { get; set; }
    [Comment("Description for Is Sent")]
    public bool IsSent { get; set; }

    [Comment("Description for Error")]
    public string? Error {  get; set; }

}
