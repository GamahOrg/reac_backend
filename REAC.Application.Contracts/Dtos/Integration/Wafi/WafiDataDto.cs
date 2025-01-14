using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Wafi
{
    public class WafiDataDto
    {
        public WafiData? data { get; set; }
        public bool isDone { get; set; }
        public string message { get; set; }
    }
    public class WafiData
    {
        public string userTypeCode { get; set; }
        public string userTypeName { get; set; }
        public Person developer { get; set; }
        public Person accountant { get; set; }
        public Person engineeringConsultant { get; set; }
    }
    public class Person
    {
        public string fullName { get; set; }
        public string nidnumber { get; set; }
        public string nationality { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string commercialRegisterNumber { get; set; }
        public string facilityName { get; set; }
    }
}
