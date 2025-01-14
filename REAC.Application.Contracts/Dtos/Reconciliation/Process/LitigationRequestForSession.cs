using REAC.Application.Contracts.Dtos.Litigation;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class LitigationRequestForSessionDto
    {
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public bool AllowAdd { get; set; }
        public List<LitigationRequestSessionCreateUpdateDto>? Sessions { get; set; }
        public List<LitigationRequestModificationRequestsDto>? Modifications { get; set; }
    }
}
