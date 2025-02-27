using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.EmployeeDtos
{
    public class SysEmployeeGetUpdateDto : BaseEntity<Guid>
    {
        public Guid DepartmentId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string IdentityNumber { get; set; }

        public string Mobile { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool? IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
