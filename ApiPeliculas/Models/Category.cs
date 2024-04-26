using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models

{
    public class Category
    {
        [Key] //CLAVE PRIMARIA
        public int Id { get; set; }

        [Required] //campo requrido
        public string Name { get; set; }
        public DateTime DateCreation { get; set; }

    }
}
