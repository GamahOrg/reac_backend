namespace Core.Dtos;

public class TokenModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool? IsLawyer { get; set; }
    public bool? IsReformer { get; set; }
    public bool? IsArbitrator { get; set; }
    public bool? IsDraftAccount { get; set; }


    public bool? IsAdmin { get; set; }
    public int? DepartmentType { get; set; }
}
