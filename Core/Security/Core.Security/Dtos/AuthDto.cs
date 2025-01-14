using Core.Dtos;

namespace Core.Security.Dtos
{
    public class AuthDto
    {
        public TokenModel Token { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
