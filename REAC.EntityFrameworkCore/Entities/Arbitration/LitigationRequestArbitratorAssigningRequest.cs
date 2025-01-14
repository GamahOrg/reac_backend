using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitratorAssigningRequest : BaseEntity<Guid>
    {
        public DateTime RequestOn { get; set; }
        public Guid LitigationRequestId { get; set; }
        public ArbitratorAssigningRequestType ArbitratorAssigningRequestTypeId { get; set; }
        public Guid? LitigationRequestArbitratorNominationListId { get; set; }
        public Guid CitizenId { get; set; }
        public LitigationRequestArbitratorAssigningRequestStatus Response { get; set; }
        public DateTime? ResponseOn { get; set; }
        public string? RejectReason { get; set; }
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ArbitratorCitizen { get; set; } = null!;
        public virtual SysArbitratorAssigningRequestType ArbitratorAssigningRequestType { get; set; } = null!;
        public virtual LitigationRequestArbitratorNominationList LitigationRequestArbitratorNominationList { get; set; } = null!;


    }
}
