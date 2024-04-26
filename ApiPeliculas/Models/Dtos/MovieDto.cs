using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public byte[] UrlImage { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }
        public enum TypeClasification { Seven, Thirteen, Sixteen, Eighteen }
        public TypeClasification Clasification { get; set; }
        public DateTime DateCreation { get; set; }

        public int categoryId { get; set; }
       

    }
}
