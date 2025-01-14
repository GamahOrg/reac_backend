using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class ScheduleSessionProcessRequestPayload
    {
        public Guid RequestId { get; set; }
        public DateTime SessionDateTime { get; set; }
    }
    public class AssigningRequestsPayload
    {
        public Guid Id { get; set; }
        public string? RejectReason { get; set; }
        public LitigationRequestReformerAssigningRequestStatus LitigationRequestReformerAssigningRequestStatus { get; set; }
    }
    public class AssigningArbitratorRequestsPayload
    {
        public Guid Id { get; set; }
        public string? RejectReason { get; set; }

        public LitigationRequestArbitratorAssigningRequestStatus LitigationRequestReformerAssigningRequestStatus { get; set; }
    }
}
