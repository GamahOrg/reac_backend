using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos
{
    public class SysLitigationRequestContractTypeGetDto : BaseEntity<LitigationRequestContractType>
    {
      public string TypeName { get; set; } = null!;
      public string CodingPrefixSymbol { get; set; } = null!;
    }
}
