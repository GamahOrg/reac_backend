namespace REAC.EntityFrameworkCore.Entities.System;

public partial class DelegationRequestCreateUpdateDto
{
    public Guid Id { get; set; }

    public int IdentityTypeId { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid SourceAccountId { get; set; }

    public Guid? AccountId { get; set; }

    public DateTime RequestedOn { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string DelegationNumber { get; set; } = null!;

    public Guid DelegationFileId { get; set; }

}
