using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } 
    }
}
