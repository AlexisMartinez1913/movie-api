using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(60, ErrorMessage = "The maximum length  are 60 characters")]
        public string Name { get; set; }
        
    }
}
