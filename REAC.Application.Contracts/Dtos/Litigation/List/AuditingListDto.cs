using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Litigation.List
{
    public class AuditingListDto : BaseEntity<Guid>
    {
        public string? RequestCode { get; set; }
        [MaxLength(100)]
        public DateTime? CreatedOn { get; set; }
        public DateTime? AssignedOn { get; set; }
        public string? FirstPartyName { get; set; }
        public string? SecondPartyName { get; set; }
        public int? PretenderRating { get; set; }
        public RequestStatus? InternalStatusId { get; set; }
        public string? InternalStatus { get; set; }

        public LitigationRequestContractType? LitigationRequestContractTypeId { get; set; }
        public string? LitigationRequestContractType { get; set; }
    }
}
