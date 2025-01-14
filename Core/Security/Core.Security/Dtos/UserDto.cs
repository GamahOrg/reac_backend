using System.ComponentModel.DataAnnotations;

namespace Core.Security.Dtos
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
