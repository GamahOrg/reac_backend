using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Wpf.System
{
    public class ResponseWorkFlowDto
    {
        public RequestStatus? InternalStatus { get; set; }

        public RequestStatus? ExternalStatus { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsFinished { get; set; }


    }
}
