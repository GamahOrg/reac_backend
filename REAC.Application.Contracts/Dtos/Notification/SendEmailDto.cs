using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Notification
{
    public class SendEmailDto
    {
        public List<string> To { get; set; }=new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
