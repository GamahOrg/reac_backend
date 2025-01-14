using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class LitigationResponseDto
    {
        public string RequestCode { get; set; }
        public Guid ContractId { get; set; }
        public int LitigationRequestCategoryId { get; set; }
        public int LitigationRequestOwnerTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsFinancialLitigation { get; set; }
        public Guid LitigationRequestAmountId { get; set; }
        public int Fees { get; set; }
        public bool IsPaid { get; set; }
        public Guid ExemptionReductionCategoryId { get; set; }
        public List<SysFileManagerCreateUpdateDto> LitigationRequestGrounds { get; set; }
        public List<LitigationRequestRequest> LitigationRequestRequests { get; set; }

    }

    public class LitigationRequestRequest
    {
        public string RequestContent { get; set; }
        public Guid Id { get; set; }
    }
}
