using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysVacationRequest : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the employee")]
        public Guid EmployeeId { get; set; }

        [Comment("Start date of the vacation")]
        public DateTime StartDate { get; set; }

        [Comment("End date of the vacation")]
        public DateTime EndDate { get; set; }

        [Comment("Reason for the vacation request")]
        public string? Reason { get; set; }

        [Comment("Indicates whether the vacation request is approved")]
        public bool? IsApproved { get; set; }

        [Comment("Unique identifier of the employee who approved the request")]
        public Guid? ApprovedBy { get; set; }

        [Comment("Date and time when the request was approved")]
        public DateTime? ApprovedOn { get; set; }

        public virtual SysEmployee SysEmployee { get; set; } = null!;

        public virtual SysEmployee? SysApprovedByEmployee { get; set; }

    }
}

