using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.EmployeeDtos
{
    public class SysEmployeeListDto : BaseEntity<Guid>
    {

        public string? DepartmentName { get; set; } 

        public string? EmployeeName { get; set; } = null!;

        public string? Mobile { get; set; } = null!;

        public bool? IsActive { get; set; }

    }
}
