using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.SysOtherCategory
{
    public class SysOtherCategoryDto:BaseEntity<Guid>
    {
        public string CategoryName { get; set; } = null!;


    }
   
}
