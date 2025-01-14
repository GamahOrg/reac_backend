using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Wafi
{
    public class WafiSecondPartyDto
    {
        public string AccountantName { get; set; }
        public string AccountantIdentityNumber { get; set; }
        public bool IsPretender { get; set; }

        [MaxLength(15)]
        public string? AgencyNumber { get; set; } // رقم الوكالة
        [MaxLength(10)]
        public string? LawLicenseNumber { get; set; }  //رقم رخصة المحاماة
        public DateTime? LawLicenseExpirationDate { get; set; }  //تاريخ انتهاء رخصة المحاماه
        public SysFileManagerCreateUpdateDto? AgancyAttachment { get; set; }
        public SysFileManagerCreateUpdateDto? LawLicenseAttachment { get; set; }
        public SysFileManagerCreateUpdateDto? EstablishmentContract { get; set; }

        public string FacilityName { get; set; }
        public string NationalAddress { get; set; }
        public string Nationality { get; set; }
        public string Name { get; set; }
        public string Capacity { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string FacilityNumber { get; set; }
        public string CommercialRegistrationNo { get; set; }
    }
}
