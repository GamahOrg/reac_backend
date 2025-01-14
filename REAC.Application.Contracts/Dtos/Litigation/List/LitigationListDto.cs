using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Litigation.List
{
    public class LitigationListDto : BaseEntity<Guid>
    {
        public string? RequestCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? FirstPartyName { get; set; }
        public string? SecondPartyName { get; set; }
        public int? PretenderRating { get; set; }
        public RequestStatus? InternalStatusId { get; set; }
        public RequestStatus? ExternalStatusId { get; set; }
        public LitigationRequestContractType? LitigationRequestContractTypeId { get; set; }
        public string? LitigationRequestContractType { get; set; }

    }
}
