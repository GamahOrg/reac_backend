namespace REAC.Application.Contracts.Dtos.Citizen;

public partial class CitizenCreateUpdateDto
{
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public int IdentityTypeId { get; set; }

    public Guid? UserId { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public Guid PassAccountId { get; set; }

    public string? Otp { get; set; }

    public int GenderId { get; set; }

    public DateTime IdentityExpiryDate { get; set; }

    public Guid? IdentityFileId { get; set; }

    public string Employer { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string? HomeTelephone { get; set; }

    public string? WorkTelephone { get; set; }

    public string? Fax { get; set; }

    public string Email { get; set; } = null!;

    public Guid AreaId { get; set; }

    public Guid CityId { get; set; }

    public Guid NationalityId { get; set; }

    public string District { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string? PostalCode { get; set; }

    public string? SubNumber { get; set; }

    public string BuildingNumber { get; set; } = null!;

    public string UnitNumber { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}
