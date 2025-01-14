using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenNationalityDto
    {
        public string NationalityName { get; set; } = null!;
        public int NationalityCode { get; set; }
    }
}
