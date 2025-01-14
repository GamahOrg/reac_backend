using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System
{
    public class FileParameterDto : BaseEntity<Guid>
    {
        public List<Guid>? List { get; set; }
    }
}
