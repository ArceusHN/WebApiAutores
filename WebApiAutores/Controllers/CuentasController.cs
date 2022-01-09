using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOS;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> manager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> manager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.manager = manager;
            this.configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var user = new IdentityUser() { Email=credencialesUsuario.Email, UserName=credencialesUsuario.Email};
            var resultado = await userManager.CreateAsync(user, credencialesUsuario.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            return await ConstruirToken(credencialesUsuario);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credenciales)
        {
            var rst = await manager.PasswordSignInAsync(credenciales.Email, credenciales.Password, isPersistent: false, lockoutOnFailure: false);
            if (rst.Succeeded)
            {
                try
                {
                    var token = await ConstruirToken(credenciales);
                    return token;
                }
                catch (Exception err)
                {
                    return Problem(err.Message);
                }
             
             
            }

            return BadRequest("Usuario y/o constraseña incorrectos");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var emailValue = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = emailValue
            };

            return await    ConstruirToken(credencialesUsuario);
        }
        
        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO adminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(adminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO adminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(adminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credenciales.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var clave = configuration["llavejwt"];
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: exp, signingCredentials: creds);

            return new RespuestaAutenticacion() { Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = exp
            };
                
        }

    }
}
