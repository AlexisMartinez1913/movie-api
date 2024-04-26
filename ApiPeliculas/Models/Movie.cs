using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] UrlImage { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public enum TypeClasification { Seven, Thirteen, Sixteen, Eighteen}
        public TypeClasification Clasification { get; set; }
        public DateTime DateCreation { get; set; }

        [ForeignKey("categoryId")]
        public int categoryId { get; set; }
        public Category Category { get; set; }



    }
}
