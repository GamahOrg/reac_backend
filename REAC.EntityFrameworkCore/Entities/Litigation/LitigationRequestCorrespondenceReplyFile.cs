using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestCorrespondenceReplyFile : BaseEntity<Guid>
    {
        public Guid LitigationRequestCorrespondenceReplyId { get; set; }

        public Guid FileManagerId { get; set; }
        public Guid? CreatedBy { get; set; }
     
        public DateTime CreatedOn { get; set; }
        public virtual SysFileManager FileManager { get; set; } = null!;

        public virtual LitigationRequestCorrespondenceReply LitigationRequestCorrespondenceReply { get; set; } = null!;
    }
}
