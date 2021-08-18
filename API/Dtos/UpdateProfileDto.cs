using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class UpdateProfileDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; }

        [Required]
        public string Bio { get; set; }
    }
}