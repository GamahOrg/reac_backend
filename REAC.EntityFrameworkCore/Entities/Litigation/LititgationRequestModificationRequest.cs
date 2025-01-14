using Azure.Core;
using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LititgationRequestModificationRequest : BaseEntity<Guid>
    {
        public string ModificationRequest { get; set; }
        public Guid LitigationRequestId { get; set; }
        public Guid LitigationRequestSessionId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual LitigationRequestSession LitigationRequestSession { get; set; } = null!;
        public virtual Citizen Citizen { get; set; } = null!;
    }
}
