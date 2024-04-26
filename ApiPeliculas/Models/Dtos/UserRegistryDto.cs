using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class UserRegistryDto
    {
        [Required(ErrorMessage = "username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
