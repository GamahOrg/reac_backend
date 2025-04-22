using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationSMS : BaseEntity<Guid>
    {
        public string Message  { get; set; }
        public string MobileNumber  { get; set; }
        public DateTime CreatedOn { get; set; }
        public int RetryAttempt { get; set; }
        public bool IsSent { get; set; }

        public string? Error { get; set; }
    }
}
