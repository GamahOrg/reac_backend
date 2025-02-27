using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestModificationRequestsDto : BaseEntity<Guid>
    {
        public string ModificationRequest { get; set; }
        public Guid LitigationRequestId { get; set; }
        public string RequestCode { get; set; }
        public Guid LitigationRequestSessionId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool? IsApproved { get; set; }
        public NotificationActorType ActorType { get; set; }
        public CitizenShortDto Citizen { get; set; }
    }
}
