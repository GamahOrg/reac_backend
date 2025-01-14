using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reconciliation
{
    public class LitigationRequestCorrespondenceReplyCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestCorrespondenceId { get; set; }

        public string ReplyMessage { get; set; }
        public Guid? CorrespondenceReplyFileId { get; set; } = null!;

    }
}
