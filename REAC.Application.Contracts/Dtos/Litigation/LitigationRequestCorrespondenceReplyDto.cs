using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestCorrespondenceReplyDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestCorrespondenceId { get; set; }

        public string ReplyMessage { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public ICollection<SysFileManagerCreateUpdateDto> LitigationRequestCorrespondenceReplyFiles { get; set; } = null!;
        public CitizenShortDto Citizen { get; set; }


    }
}
