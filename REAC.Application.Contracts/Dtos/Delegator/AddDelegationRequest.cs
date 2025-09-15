using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class AddDelegationRequest
    {
        [Required]
        public Guid LitigationRequestId { get; set; }
        public LitigationRequestParty LitigationRequestParty { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
        public LitigationRequestDelegationDataDto LitigationRequestDelegationData { get; set; }
    }
}
