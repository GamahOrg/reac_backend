using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class MOJRequestDto
    {
        public int AttorneyCode { get; set; }
        public string FirstParties { get; set; }
        public string SecondParties { get; set; }
    }

}
