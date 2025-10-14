using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class ChangeCitizenData
    {
        public Guid CitizenId { get; set; }
        public Guid LitigationRequestId { get; set; }
        public string? Mobile2 { get; set; }
        public string? Email2 { get; set; }
    }
}
