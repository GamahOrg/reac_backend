namespace REAC.EntityFrameworkCore.Entities.System;

public partial class FacilityShortDto
{
    public Guid Id { get; set; }

    public string? Code { get; set; }
    public string ArabicFacilityName { get; set; } = null!;

    public string EnglishFacilityName { get; set; } = null!;

    public string Facility700Number { get; set; } = null!;

    public string? CommercialRegisterNumber { get; set; }

}
