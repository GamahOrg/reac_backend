using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class GeneralGEEErrorDto
    {
        public string SourceAgency { get; set; }
        public string ErrorType { get; set; }
        public string? RaisedBy { get; set; }
        public string Code { get; set; }
        public string ErrorText { get; set; }

    }
}
