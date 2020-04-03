using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8,MinimumLength = 4, ErrorMessage = "El password debe contener entre 4 y 8 caracteres")]
        public string Password { get; set; }
    }
}