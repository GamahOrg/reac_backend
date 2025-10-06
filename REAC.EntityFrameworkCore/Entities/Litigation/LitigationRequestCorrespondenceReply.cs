using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestCorrespondenceReply : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request correspondence")]
        public Guid LitigationRequestCorrespondenceId { get; set; }

        [Comment("Description for Reply Message")]
        public string ReplyMessage { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime CreatedOn { get; set; }
        [Comment("Description for Created By")]
        public Guid CreatedBy { get; set; }

        public virtual LitigationRequestCorrespondence LitigationRequestCorrespondence { get; set; } 
        public virtual Citizen Citizen { get; set; }

        public virtual ICollection<LitigationRequestCorrespondenceReplyFile> LitigationRequestCorrespondenceReplyFiles { get; set; } = new List<LitigationRequestCorrespondenceReplyFile>();


    }
}

