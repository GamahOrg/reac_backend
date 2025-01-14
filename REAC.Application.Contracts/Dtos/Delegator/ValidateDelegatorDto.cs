using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class ValidateDelegatorDto
    {
        public int DelegateNumber { get; set; }
        public string IdentityNumber { get; set; }
    }
    
    public class ValidateLawyerDelegatorDto
    {
        public string LawyerLicenseNumber { get; set; }
        public string IdentityNumber { get; set; }
    }
}
