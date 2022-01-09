namespace WebApiAutores.DTOS.Autor
{
    public class AutorDTOConLibros : AutorDTO
    {
        public List<LibroDTO> Libros { get; set; }
    }
}
