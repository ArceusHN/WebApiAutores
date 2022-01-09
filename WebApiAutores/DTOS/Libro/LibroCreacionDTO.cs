using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOS
{
    public class LibroCreacionDTO
    {
        [Required]
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
