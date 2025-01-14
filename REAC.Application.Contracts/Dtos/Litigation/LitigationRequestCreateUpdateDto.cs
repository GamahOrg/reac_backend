using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Litigation;

public partial class LitigationRequestCreateUpdateDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }  // رقم الدعوي

    public Guid ContractId { get; set; } 

    public Guid ApplicantAccountId { get; set; }

    public int LitigationRequestTypeId { get; set; } 

    public int LitigationRequestCategoryId { get; set; }// نوع الطلب

    public int LitigationRequestOwnerTypeId { get; set; }  // مقدم الدعوي

    public int InternalStatusId { get; set; }

    public int ExternalStatusId { get; set; }

    public string? Title { get; set; } // عنوان الدعوي

    public string? Description { get; set; } // وصف الدعوي

    public bool? IsFinancialLitigation { get; set; } // نزاع مالي ام لا 

    public Guid LitigationRequestAmountId { get; set; }// مبلغ المنازعه

    public decimal Fees { get; set; }    // الرسوم الادارية

    public bool IsPaid { get; set; }

    public DateTime? PaidOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsSent { get; set; }

    public DateTime? SentOn { get; set; }
    public Guid RegionId { get; set; }
    public Guid CityId { get; set; }
    public string? LitigationPurpose { get; set; } // الطلبات
    public string? BondTitle { get; set; } //عنوان السند
    public string? AssociationName { get; set; } // اسم الجمعية
    public string? AssociationCode { get; set; } // كود الجمعية
    public Guid? ExemptCategoryId { get; set; } // اذا كان من الفئات المعفاه

}
