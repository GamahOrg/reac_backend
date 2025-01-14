using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Reconciliation
{
    public class LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }
        public LitigationRequestExtendPeriodRequestStatus? LitigationRequestExtendPeriodRequestStatusId { get; set; }

        public string? FinishStatusReason { get; set; }

    }
}
