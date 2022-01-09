using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOS;
using WebApiAutores.DTOS.Libro;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<LibroDTO>>> Get()
        {
            var libro = await _db.Libros.ToListAsync();
            return _mapper.Map<List<LibroDTO>>(libro);
        }

        [HttpGet("{id:int}", Name ="ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await _db.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibro => autorLibro.Autor)
                .FirstOrDefaultAsync(libro => libro.Id == id);

            if(libro == null)
            {
                return NotFound();
            }

            if(libro.AutoresLibros != null)
            {
                libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();   
            }

            return _mapper.Map<LibroDTOConAutores>(libro);
        }       

        [HttpPost]
        public async Task<IActionResult> Post(LibroCreacionDTO libro)
        {

            if(libro.AutoresIds == null || libro.AutoresIds.Count == 0)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await _db.Autores.Where(autorDB => libro.AutoresIds.Contains(autorDB.Id))
                .Select(autorDB => autorDB.Id).ToListAsync();

            if (libro.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest($"No existe uno de los autores enviados");
            }

            var libroMappeado = _mapper.Map<Libro>(libro);
            AsignarOrdenAutores(libroMappeado);

            _db.Libros.Add(libroMappeado);
            await _db.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libroMappeado);

            return CreatedAtRoute("ObtenerLibro", new {id=libroMappeado.Id}, libroDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(LibroCreacionDTO libroCreacionDTO, int id)
        {
            var libroDB = await _db.Libros.Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x=> x.Id == id);

            if (libroDB == null) return NotFound();

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await _db.SaveChangesAsync();
            return NotFound();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var libroDB = await _db.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null) return NotFound();

            var libroDTO = _mapper.Map<LibroPatchDTO>(libroDB);
            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);
            if (!esValido) return BadRequest(ModelState);

            _mapper.Map(patchDocument, libroDB);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

    }
}
