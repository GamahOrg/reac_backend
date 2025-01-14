
using Microsoft.AspNetCore.Identity;

namespace Core.Security.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }

        public Guid? ReferenceId { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
