using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.VacationDtos;

public partial class SysVacationRequestCreateUpdateDto : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Reason { get; set; }

    public bool? IsApproved { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
