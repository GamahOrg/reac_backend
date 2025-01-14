using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Integration.Party
{
    public class PartyDTO
    {
        public string? IdentityNumber { get; set; }
        public Guid CitizenId { get; set; }
        public string? NationalAddress { get; set; }
        public string? CommercialRegistrationNo { get; set; }
        public string? FacilityName { get; set; }
        public string? Nationality { get; set; }
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? FacilityNumber { get; set; }
        public string? CommercialRegisterNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Capacity { get; set; }
        public bool IsPretender { get; set; } = false; // مدعي ولا لا
        [MaxLength(15)]
        public string? AgencyNumber { get; set; } // رقم الوكالة
        [MaxLength(10)]
        public string? LawLicenseNumber { get; set; }  //رقم رخصة المحاماة
        public DateTime? LawLicenseExpirationDate { get; set; }  //تاريخ انتهاء رخصة المحاماه

        public SysFileManagerCreateUpdateDto? AgancyAttachment { get; set; }
        public SysFileManagerCreateUpdateDto? LawLicenseAttachment { get; set; }
        public SysFileManagerCreateUpdateDto? EstablishmentContract { get; set; }
    }
}
