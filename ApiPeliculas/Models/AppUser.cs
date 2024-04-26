using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Models
{
    public class AppUser : IdentityUser
    {
        //campos personalizados
        
        public string Name { get; set; }
    }
}
