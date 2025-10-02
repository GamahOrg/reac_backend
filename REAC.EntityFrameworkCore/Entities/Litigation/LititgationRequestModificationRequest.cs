using Azure.Core;
using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LititgationRequestModificationRequest : BaseEntity<Guid>
    {
        [Comment("Description for Modification Request")]
        public string ModificationRequest { get; set; }
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Unique identifier of the litigation request session")]
        public Guid? LitigationRequestSessionId { get; set; }
        [Comment("Description for Created By")]
        public Guid CreatedBy { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime CreatedOn { get; set; }
        [Comment("Description for Is Approved")]
        public bool? IsApproved  { get; set; }
        [Comment("Description for Actor Type")]
        public NotificationActorType? ActorType  { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual LitigationRequestSession LitigationRequestSession { get; set; } = null!;
        public virtual Citizen Citizen { get; set; } = null!;
    }
}

