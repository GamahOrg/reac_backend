using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Litigation;

public partial class LitigationRequestContractCreateUpdateDto : BaseEntity<Guid>
{
    public int LitigationRequestContractTypeId { get; set; }  // نوع الدعوي

    public string Key { get; set; } = null!;

    public string SubKey { get; set; } = null!; // رقم النسخة

    public Guid? ContractFileId { get; set; }//رقم عقد الايجار

    public Guid FirstPartyAccountId { get; set; }

    public Guid? FirstPartyFacilityId { get; set; }

    public string FirstPartyCapacity { get; set; } = null!; // صفة المدعي

    public Guid? SecondPartyAccountId { get; set; }

    public Guid? SecondPartyFacilityId { get; set; }  // المنشاة

    public string? SecondPartyCapacity { get; set; } //صفة المدعي عليه
    public string? SecondPartyName { get; set; } // اسم المدعي عليه
    public string? SecondPartyPhoneNumber { get; set; } // رقم الجوال

    public Guid? RegistrationRequestId { get; set; } //رقم السجل التجاري

    public string? DelegationOwnerName { get; set; } // اسم صاحب التفويض
    public string? IdentityType { get; set; } // نوع الهوية
    public string? IdentityNumber { get; set; } // رقم الهوية
    public DateTime? IdentityExpirationDate { get; set; } // تاريخ انتهاء الهوية
    public byte[]? IdentityPhoto { get; set; }

    // طرفي العقد 
    // طرفي الدعوي

}
