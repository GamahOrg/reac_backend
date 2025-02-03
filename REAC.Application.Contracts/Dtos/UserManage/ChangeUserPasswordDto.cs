using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.UserManage
{
    public class ChangeUserPasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
