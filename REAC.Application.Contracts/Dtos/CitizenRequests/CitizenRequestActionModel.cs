using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public class CitizenRequestActionModel
    {
        public CitizenRequestAction CitizenRequestAction { get; set; }
        public Guid RequestId { get; set; }
        public string? Notes { get; set; }
    }
}
