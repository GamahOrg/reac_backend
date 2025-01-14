using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Wafi
{
    public class WafiLoginDto
    {
        public WafiLoginData data { get; set; }
        public bool isDone { get; set; }
        public string message { get; set; }
    }

   public class WafiLoginData
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public bool isDelegator { get; set; }
        public int userType { get; set; }

    }
}
