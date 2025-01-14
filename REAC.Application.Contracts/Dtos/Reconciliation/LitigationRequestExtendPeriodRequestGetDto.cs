using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Application.Contracts.Dtos.Reconciliation
{
    public partial class LitigationRequestExtendPeriodRequestGetDto : AuditedBaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public int RequiredDays { get; set; }
        public string RequestReason { get; set; } = null!;


        public string? Reason { get; set; }

        public string? Notes { get; set; }

        public int LitigationRequestExtendPeriodRequestStatusId { get; set; }

        public string? FinishStatusReason { get; set; }

        public List<LitigationRequestExtendPeriodRequestFileCreateUpdateDto> LitigationRequestExtendPeriodRequestFiles { get; set; }

    }
}
