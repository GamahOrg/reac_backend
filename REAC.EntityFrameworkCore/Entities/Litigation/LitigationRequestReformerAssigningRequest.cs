using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public partial class LitigationRequestReformerAssigningRequest : BaseEntity<Guid>
    {
        [Comment("Date when the reformer assignment request was submitted")]
        public DateTime? RequestOn { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid CitizenId { get; set; }
        [Comment("Description for Response")]
        public LitigationRequestReformerAssigningRequestStatus Response { get; set; }
        [Comment("Date when the reformer assignment request was responded to")]
        public DateTime? ResponseOn { get; set; }
        [Comment("Description for Reject Reason")]
        public string? RejectReason { get; set; }
        [Comment("Description for Reason")]
        public string? Reason { get; set; } 


        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ReformerCitizen { get; set; } = null!;

    }


}

