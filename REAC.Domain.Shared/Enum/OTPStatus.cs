using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Enum
{
    public enum OTPStatus
    {
        Expired = 0,
        Valid = 1,
        HasIssue = 2,
        NotExist = 3,
        SMS_Sent_Success = 4,
        SMS_Sent_Failed = 5,
        Wrong = 6,
    }
}
