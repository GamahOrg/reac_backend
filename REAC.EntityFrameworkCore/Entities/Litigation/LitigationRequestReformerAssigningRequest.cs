using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public partial class LitigationRequestReformerAssigningRequest : BaseEntity<Guid>
    {
        public DateTime? RequestOn { get; set; }
        public Guid LitigationRequestId { get; set; }
        public Guid CitizenId { get; set; }
        public LitigationRequestReformerAssigningRequestStatus Response { get; set; }
        public DateTime? ResponseOn { get; set; }
        public string? RejectReason { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual Citizen ReformerCitizen { get; set; } = null!;

    }


}
