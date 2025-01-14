using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Domain.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Litigation.Create.Litigation
{
    public class LitigationCreateUpdateDto : BaseEntity<Guid>
    {
        public string? RequestCode { get; set; } // from system
        [MaxLength(100)]
        public string? Title { get; set; } // عنوان الدعويg
        [MaxLength(1000)]
        public string? Description { get; set; } // وصف الدعوي
        public bool? IsFinancialLitigation { get; set; } // نزاع مالي ام لا 
        public Guid? LitigationRequestAmountId { get; set; }// مبلغ المنازعه
        public List<LitigationRequests>? Requests { get; set; }
        public List<LitigationReferences>? References { get; set; }
        public RequestStatus? InternalStatusId { get; set; }
        public RequestStatus? ExternalStatusId { get; set; }
        public Guid ContractId { get; set; }// contract primary key
        public decimal? Fees { get; set; } // from system
        public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
        public ExemptionReductionCategoryDto? ExemptionReductionCategory { get; set; }
        public LitigationRequestContractType LitigationRequestContractType { get; set; } // front send type here
        public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }// front send type here  from integration and check identity from system 
        public bool IsSent { get; set; }
        public DateTime? SentOn { get; set; }
        public Guid? CityId { get; set; }
        public Guid? AreaId { get; set; }
        public string? ContractAddress { get; set; } // العنوان 
        public Guid? OtherCategories { get; set; }

        //public bool isAgreedOnAcknowledgment { get; set; }
        //public bool isAgreedOnTermsAndConditions { get; set; }



    }

}
