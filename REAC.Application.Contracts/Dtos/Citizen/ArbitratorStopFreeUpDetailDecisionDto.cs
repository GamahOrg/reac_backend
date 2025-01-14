using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class ArbitratorStopFreeUpDetailDecisionDto : BaseEntity<int>
    {
        public string? ArbitratorStatus { get; set; }
        public DateTime DecisionDate { get; set; }
        public StoppingReason? StoppingReason { get; set; }
        public string? LateLitigationRequestNumber { get; set; }
    }
}
