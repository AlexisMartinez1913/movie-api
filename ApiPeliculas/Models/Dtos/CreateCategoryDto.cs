using System.ComponentModel.DataAnnotations;
namespace ApiPeliculas.Models.Dtos
{
    public class CreateCategoryDto
    {
        //validaciones para la creacion de categorias
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(80, ErrorMessage = "The maximum length  are 80 characters")]
        public string Name { get; set; }
    }
}
