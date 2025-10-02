using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorAssigningRequest : BaseEntity<Guid>
    {
        [Comment("Date when the arbitrator assignment request was submitted")]
        public DateTime RequestOn { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public ArbitratorAssigningRequestType ArbitratorAssigningRequestTypeId { get; set; }
        public Guid? LitigationRequestArbitratorNominationListId { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid CitizenId { get; set; }
        [Comment("Description for Response")]
        public LitigationRequestArbitratorAssigningRequestStatus Response { get; set; }
        [Comment("Date when the arbitrator assignment request was responded to")]
        public DateTime? ResponseOn { get; set; }
        [Comment("Description for Reject Reason")]
        public string? RejectReason { get; set; }
        public Boolean? ResponseArbitrationMinutes { get; set; }
        public DateTime? ArbitrationMinutesOn { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual SysArbitratorAssigningRequestType ArbitratorAssigningRequestType { get; set; } = null!;
        public virtual LitigationRequestArbitratorNominationList LitigationRequestArbitratorNominationList { get; set; } = null!;



    }
}

