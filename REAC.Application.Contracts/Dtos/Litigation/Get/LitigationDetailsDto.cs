using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Litigation.Get
{
    public class LitigationDetailsDto : BaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public string? RequestCode { get; set; }
        public Guid? CityId { get; set; }
        public Guid? AreaId { get; set; }
        public string? ContractAddress { get; set; }
        public bool? IsFinancialLitigation { get; set; }
        public Guid? LitigationRequestAmountId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Fees { get; set; }
        public Guid ContractId { get; set; }
        public LitigationRequestContractType LitigationRequestContractTypeId { get; set; }
        public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }

        public virtual SysFileManagerCreateUpdateDto? ExemptionReductionFile { get; set; }
        public List<LitigationRequestSessionCreateUpdateDto> LitigationRequestSessions { get; set; } = new List<LitigationRequestSessionCreateUpdateDto>();


        public List<LitigationRequestGroundCreateUpdateDto> References { get; set; } = new List<LitigationRequestGroundCreateUpdateDto>();
        public SysExemptionReductionCategoryCreateUpdateDto? ExemptionReductionCategory { get; set; } = new SysExemptionReductionCategoryCreateUpdateDto();

        public List<LitigationRequestRequestCreateUpdateDto> Requests { get; set; } = new List<LitigationRequestRequestCreateUpdateDto>();

    }
}
