using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class LitigationRequestAction
    {
        public LitigationRequestDecision LitigationRequestDecision { get; set; }
        public Guid RequestId { get; set; }
    }
}
