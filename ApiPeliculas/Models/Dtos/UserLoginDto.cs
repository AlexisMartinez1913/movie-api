using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "username is required")]
        public string UserName {  get; set; }
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
    }
}
