namespace REAC.EntityFrameworkCore.Entities.System;

public partial class FacilityShortDto
{
    public Guid Id { get; set; }

    public string? Code { get; set; }
    public string ArabicFacilityName { get; set; } = null!;

    public string EnglishFacilityName { get; set; } = null!;

    public string Facility700Number { get; set; } = null!;

    public string? CommercialRegisterNumber { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? IdentityNumber { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Street { get; set; }

}
