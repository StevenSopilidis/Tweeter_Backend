using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}