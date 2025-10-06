using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationSMS : BaseEntity<Guid>
    {
        [Comment("Description for Message")]
        public string Message  { get; set; }
        [Comment("Number of the mobile")]
        public string MobileNumber  { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime CreatedOn { get; set; }
        [Comment("Description for Retry Attempt")]
        public int RetryAttempt { get; set; }
        [Comment("Description for Is Sent")]
        public bool IsSent { get; set; }

        [Comment("Description for Error")]
        public string? Error { get; set; }
    }
}

