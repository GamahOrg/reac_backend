using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reports
{
    public class GenericEmployeeReportsFilter
    {
        public string? ReviewerName { get; set; }
        public int? ReviewerStatus { get; set; }

        public string? ArbitratorName { get; set; }
        public int? ArbitratorStatus { get; set; }

        public string? ReformerName { get; set; } 
       public int? ReformerStatus { get; set; }

        public int? ExperienceYears { get; set; }


        public int? Skip { get; set; }
        public int? Take { get; set; }
        //public bool? IsAdmin { get; set; }
        public string? SortDirection { get; set; }
    }
}
