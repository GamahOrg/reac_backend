
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Security.Entities
{
    [Table("AspNetUsers")]
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }

        public Guid? ReferenceId { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
