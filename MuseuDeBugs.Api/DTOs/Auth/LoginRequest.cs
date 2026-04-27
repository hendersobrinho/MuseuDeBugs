using System.ComponentModel.DataAnnotations;

namespace MuseuDeBugs.Api.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(80)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Password { get; set; } = string.Empty;
    }
}
