using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestAmountDtos
{
    public class SysLitigationRequestAmountGetDto : BaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public string LitigationAmount { get; set; }
    }
}
