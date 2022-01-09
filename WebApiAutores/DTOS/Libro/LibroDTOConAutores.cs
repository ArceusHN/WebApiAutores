namespace WebApiAutores.DTOS.Libro
{
    public class LibroDTOConAutores : LibroDTO
    {
        public List<AutorDTO> Autores { get; set; }
    }
}
