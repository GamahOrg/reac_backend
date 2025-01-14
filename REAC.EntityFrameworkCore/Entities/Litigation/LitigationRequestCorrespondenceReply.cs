using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestCorrespondenceReply : BaseEntity<Guid>
    {
        public Guid LitigationRequestCorrespondenceId { get; set; }

        public string ReplyMessage { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

        public virtual LitigationRequestCorrespondence LitigationRequestCorrespondence { get; set; } 
        public virtual Citizen Citizen { get; set; }

        public virtual ICollection<LitigationRequestCorrespondenceReplyFile> LitigationRequestCorrespondenceReplyFiles { get; set; } = new List<LitigationRequestCorrespondenceReplyFile>();


    }
}
