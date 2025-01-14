using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.Create.Defendant
{
    public class DefendantCreateUpdateDto :BaseEntity<Guid>
    {
        // id is contract primary Id
        public string SecondPartyName { get; set; }
        public string SecondPartyIdentityNumber { get; set; }
        public string SecondPartyMobile { get; set; } = null!;
        public Guid? SecondPartyFacilityId { get; set; }  // المنشاة
        //public Guid? RegistrationRequestId { get; set; } = null; //رقم السجل التجاري
        public string SecondPartyCapacity { get; set; } //صفة المدعي عليه
        public string SecondPartyNationality { get; set; }  
        public string? NationalAddress { get; set; }  
        public string? Email { get; set; }  
        public string? Mobile { get; set; }  
        public DateTime? Birthdate { get; set; }  
        public string? CommercialRegistrationNumber { get; set; }  

    }
}
