using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.AspNetCore.Http;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Contract
{
    public class OtherCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid? LeaseContractFileId { get; set; }
        [MaxLength(12)]
        public string ContractKey { get; set; }//رقم عقد الايجار
        [MaxLength(4)]
        //  public string VersionNumber { get; set; } = null!; // رقم النسخة
        public List<string> OtherCategories { get; set; }
        public LitigationRequestOwnerType FirstPartyCapacity { get; set; } // صفة المدعي
        public Guid ContractAttachmentId { get; set; }  // عقد ايجار محل الدعوة
       // public IFormFile? IdentityAttachment { get; set; } // صورة الهوية PENDING
        public string IdentityNo { get; set; }
        public Guid FirstPartyCitizenId { get; set; }
        public string NationalAddress { get; set; }
        public string CommercialRegistrationNo { get; set; }
        public string FacilityName { get; set; }
        public string Nationality { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FacilityNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Capacity { get; set; }
     //   public bool IsPretender { get; set; } = false; // مدعي ولا لا 

        [MaxLength(15)]
        public string? AgencyNumber { get; set; } // رقم الوكالة
        [MaxLength(10)]
        public string? LawLicenseNumber { get; set; }  //رقم رخصة المحاماة
        public DateTime? LawLicenseExpirationDate { get; set; }  //تاريخ انتهاء رخصة المحاماه
        public Guid AgancyAttachmentId { get; set; }
        public Guid LawLicenseAttachmentId { get; set; }
        public Guid EstablishmentContractId { get; set; }

    }
}
