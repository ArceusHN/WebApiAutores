using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOS;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")] // Cada comentario depende de un libro
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet("{id:int}", Name ="ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentario = await _context.Comentarios
                .FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null) return NotFound();

            return _mapper.Map<ComentarioDTO>(comentario);

        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var comentario = await _context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();   
            return _mapper.Map<List<ComentarioDTO>>(comentario);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO creacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var emailClaimValue = emailClaim.Value;
            var libro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            var usuario = await userManager.FindByEmailAsync(emailClaimValue);
            var usuarioId = usuario.Id;

            if (usuario == null)

            if(!libro)
            {
                return NotFound();
            }
            var comentario = _mapper.Map<Comentario>(creacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, ComentarioCreacionDTO comentarioCreacionDTO,int id)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro) return NotFound();

            var existeComentario = await _context.Libros.AnyAsync(c => c.Id == id);

            if (!existeComentario) return NotFound();

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;

            _context.Update(comentario);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
