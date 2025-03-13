using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Wpf.Business
{
    public class AssignRequestPayload
    {
        public Guid RequestId { get; set; }

        public Guid ReformerId { get; set; }
        public string? Reason { get; set; }
    }

    public class AssignReviewerRequestPayload
    {
        public Guid RequestId { get; set; }

        public Guid EmployeeId { get; set; }
    }
    public class AssignArbitratorRequestPayload
    {
        public Guid RequestId { get; set; }
        public Guid LitigationRequestArbitratorListId { get; set; }
        public ArbitratorAssigningRequestType ArbitratorAssigningRequestTypeId { get; set; }
        public Guid ArbitratorId { get; set; }
    }
}
