using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class GetDeligatorCitizenData
    {
        public int DelegateNumber { get; set; }
        public string DelegatorIdentityNumber { get; set; }
        public string UserIdentityNumber { get; set; }
    }
}
