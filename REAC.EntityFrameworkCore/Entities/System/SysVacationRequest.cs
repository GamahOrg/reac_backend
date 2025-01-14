using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysVacationRequest : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public bool? IsApproved { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public virtual SysEmployee SysEmployee { get; set; } = null!;

        public virtual SysEmployee? SysApprovedByEmployee { get; set; }

    }
}
