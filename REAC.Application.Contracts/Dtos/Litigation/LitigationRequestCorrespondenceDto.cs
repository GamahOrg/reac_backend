using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestCorrespondenceDto
    {
        public Guid Id { get; set; }
        public Guid LitigationRequestId { get; set; }

        public LitigationRequestCorrespondenceDestination LitigationRequestCorrespondenceDestinationId { get; set; }

        public string? Subject { get; set; }
        public string SerialCode { get; set; }

        public string? Message { get; set; }
        public bool AllowAddReply { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public ICollection<SysFileManagerCreateUpdateDto> LitigationRequestCorrespondenceFiles { get; set; } = null!;
        public ICollection<LitigationRequestCorrespondenceReplyDto> LitigationRequestCorrespondenceReply { get; set; } = null!;

        public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
        public SysLitigationRequestCorrespondenceStatus LitigationRequestCorrespondenceStatus { get; set; }

        public CitizenShortDto Citizen { get; set; }

    }
}
