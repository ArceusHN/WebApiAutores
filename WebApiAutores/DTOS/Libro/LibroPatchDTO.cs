using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOS.Libro
{
    public class LibroPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
