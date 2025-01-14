using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class GEELoginDto
    {
        public string LoginURL { get; set; }
        public GEELoginAuthorizationDto authorizationDto { get; set; }
    }

    public class GEELoginAuthorizationDto
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class GEELoginResponseDto
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
    }
}
