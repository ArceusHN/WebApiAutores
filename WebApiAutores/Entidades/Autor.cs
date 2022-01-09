using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El campo {0} es requerido")]
        //[PrimeraLetraMayusculaAttribute]
        public string Nombre { get; set; }
        public List<Libro> Libros { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

        [NotMapped]
        public int Menor { get; set; }

        [NotMapped]
        public int Mayor { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("The first letter must be capitalized!", 
                        new string[] {nameof(Nombre)});
                }


            }

            if(Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser mas grande que el campo mayor",
                    new string[] { nameof(Menor) });
            }

        }
    }


}
