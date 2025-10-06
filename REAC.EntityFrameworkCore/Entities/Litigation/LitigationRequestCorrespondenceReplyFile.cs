using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestCorrespondenceReplyFile : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request correspondence reply")]
        public Guid LitigationRequestCorrespondenceReplyId { get; set; }

        [Comment("Unique identifier of the file manager")]
        public Guid FileManagerId { get; set; }
        [Comment("Description for Created By")]
        public Guid? CreatedBy { get; set; }
     
        [Comment("Date and time when the record was created")]
        public DateTime CreatedOn { get; set; }
        public virtual SysFileManager FileManager { get; set; } = null!;

        public virtual LitigationRequestCorrespondenceReply LitigationRequestCorrespondenceReply { get; set; } = null!;
    }
}

