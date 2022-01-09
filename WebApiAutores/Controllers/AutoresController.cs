using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiAutores.DTOS;
using WebApiAutores.DTOS.Autor;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    #region Introduccion API Rest
    // La idea fundamental de tener un WEB API es que tendremos clientes que haran peticiones HTTP
    // a nuestro WEB API
    // EJEMPLO DE RUTA:
    // https://miapi.com/autores | miapi.com es el dominio | /autores es el recurso.(o bien la ruta)

    //- Una accion(endpoint) es una funcion de un controlador que se ejecua en respuesta a una peticion
    //    HTTP realizada a nuestro WEB API.
    //- Un controlador es una clase que agrupa un conjunto de acciones | agrupa acciones relacionadas a un
    //    recurso. 
    //- Controlador: Nomenclatura: Nombre + Controller -> AutoresController.
    //- Podemos configurar que accion se ejecutará en funcion de la ruta y el metodo HTTP.
    #endregion

    #region Reglas de Ruteo
    //  Las reglas de ruteo nos permiten maperar una URL o una RUTA con una accion o endpoint, es decir, al hacer una peticion
    // hacia una ruta: api/autores/1  <- Hay un mecanismo de reglas de ruteo en ASP que permite hacer una relacion entre
    // la ruta y una accion del controlador.

    // //Variables o Parametros de Ruta 
    // [HTTPGet("listado")] <- Sabemos que se le concatena a la ruta la palabra "listado"


    //En el caso de que queramos un valor variable que no sea en duro tal como el "listado" se usan los parametros de ruta.

    // [HTTPGet({ ID})] <- Tambien lo conocemos como plantilla de la ruta: midominio.com/api/autores/1
    #endregion

    #region Tipos de dato de retorno
    //Se clafisican en tres:
    //   - Tipo de dato en especifico EJEMPLO: public List<Autor> Get();
    //   - ActionResult<T> EJEMPLO: public ActionResult<List<Autor>> Get(); Permite retornar una de dos cosas:
    //                      <T> Vease T como cualquier tipo de dato que se especifique o retornar cualquier tipo de
    //                       dato que herede de ActionResult.
    //   - IActionResult EJEMPLO: public IActionResult Get

    #endregion

    #region Programacion Asincrona
    //- Es recomendable la programacion asincrona cuando se trabaja con una DB
    // - Debemos utilizar la programacion asincrona cuando realizamos operaciones I/O:
    //     * Las operaciones I/O se caracterizan por la comunicacion con sistemas externos o terceros al nuestro.
    //     EJEMPLOS:
    //     -Peticiones a un web service,
    //     -Interaccion con una DB,
    //     -Interaccion con un sistema de archivos.
    //     Al llamar a un tercero se debe esperar por una respuesta, durante la espera es productivo liberar el Hilo
    //     que inicio la operacion para que este proceda a realizar otras tareas.

    // - Una regla para utilizar correctamente la asincronia debemos de retornar de un metodo asincrono:
    //  Task o ValueTask
    //  Ejemplo:
    //     public async Task<ActionResult<Autor>> Get(int id)
    //     Task -> Representa una promesa(Osea un valor que va a ser retornado en el futuro)
    //     Es lo que permite que c# haga otras tareas mientras se espera por la resolucion de 
    //     la operacion IO.

    // -Task -> Seria como retornar void
    // -Task<T> Es para retornar en el futuro T
    // - la palabra reservada "await" espera poder trabajar con un Task.
    #endregion

    #region Model Binding
    //- Nos permite mapear datos de una peticion HTTP a los parametros de un endpoint o accion.
    //-Podemos tener varias fuentes de donde pueden venir los valores de los parametros de nuestras
    // acciones o endopoints.
    //Pueden venir de:
    //   - Query Strings ->  [FromQuery]
    //   - Headers o Cabeceras -> [FromHeader]
    //   - Body o Cuerpo de la peticion -> [FromBody]
    //   - Ruta o Parametros de ruta -> [FromRoute]
    //   - Servicios -> [FromServices]
    //   - Form - Cuando la fuente viene con el content-type-> url-formencoded -> [FromForm]
    #endregion

    #region Validacion de reglas de modelo
    //   Queremos tener un conjunto de reglas que nos permitan validar que los modelos
    //   esten correctos cuando los usuarios nos envien la data del respectivo modelo.

    //   // Validaciones por Atributo
    //    - Se pueden hacer mediante data annotations ya definidos.
    //    - Podemos crear nuestros propios atributos de validacion (Customs o Personalizados
    //        a travez de la clase ValidationAttribute

    //  // Validacion por Modelo
    //     A travez de IValidatable Object
    //    -Observacion: 
    //    - Para que las validaciones a nivel de MODELO se ejecuten
    //        se debe pasar primer todas las reglas de validacion
    //        a nivel de atributo!.

    //  // Regla de validacion a nivel del controlador -> accion
    //    Ejemplo:
    //    Antes de agregar un nuevo autor, se puede validar con una
    //    consulta a la DB que el autor que se quiera agregar no exista
    //    previamente
    #endregion

    #region Inyeccion de Dependencias
    // Las clases de nuestra app rara vez son completamente autosuficientes. Es normal separar responsabilidades
    // entre distintas clases.
    // Lo mismo ocurre con nuestros controladores. Un controlador contiene un conjunto de endpoints o acciones por lo que la responsabilidad
    // del controlador es recibir y procesar peticiones HTTP realizadas hacia nuestro WEB API.
    // Un controlador no debe tener la responsabilidad de guardar registros en DB, ni escribir mensajes en consola, ni realizar otras tareas.
    // Dichas tareas deben ser delegadas a otra clases. 
    // DEBEMOS DE TOMER SIEMPRE EN CONSIDERACION EL PRINCIO DE RESPONSABILIDAD UNICA (SRP).

    // Cuando una clase A utiliza una clase B, decimos que B es una DEPENDENCIA de la clase A, por lo tanto, la clase A tiene una dependencia con  
    // la clase B.
    // Las dependencias son inevitables y han sido estudiadas en software y se utiliza un termino para evaluarlas:
    // ACOMPLAMIENTO: El acoplamiento puede ser de dos maneras
    // Acomplamiento Alto: Se caracteriza por una dependencia poco flexible en otra clases. No es bueno en general.
    // Acomplamiento Bajo: Se caracteriza por una dependencia flexible.

    #region EJEMPLO DE ACOPLAMIENTO ALTO
    // La relacion entre Autores y ApplicationDbContext esta completamente oculta o invisible del cliente de la clase
    // debido a que estamos especificando el context dentro de un metodo y no en el constructor.
    // Estonces se desconoce que la clase Autores tiene depencia con el ApplicationDbContext.
    // Este es un problema por dos razones:
    // 1. Resultados inesperados en caso que haya problemas con la clase ApplicationDbContext. En tiempo de ejecucion te daras cuenta del problema.
    // 2. No hay forma de cambiar la clase con la cual AutoresController tiene dependencia. Es inflexible.
    //public AutoresController()
    //{

    //}

    //public async Task<ActionResult<List<Autor>>> Get()
    //{
    //    ApplicationDbContext context = new ApplicationDbContext(null); // No hay forma de personalizar esto sin modificar la clase AutoresController.
    //    return await context.Autores.ToListAsync();
    //}
    #endregion

    #region EJEMPLO DE ACOPLAMIENTO MEDIO
    // A pesar de que ahora la dependencia se inyecta en el constructor estos lo hace mas o menos flexible.
    // El problema radica es que el parametro sigue siendo una clase, entonces siempre tendriamos que pasarle
    // una instancias de tipo ApplicationDbContext.

    //public AutoresController(ApplicationDbContext db)
    //{
    //    _db = db;
    //}
    #endregion

    #region EJEMPLO DE ACOPLAMIENTO BAJO
    //private readonly IServicio _servicio;
    // Ahora dado que la dependencia de AutoresController es con IServicio, puedes enviarle cualquier clase que implemente IServicio.
    // Lo importante de este asunto es que ahora la clase AutoresController tiene tanta flexibilidad que yo puedo pasarle cualquier clase
    // que implemente la interfaz IServicio.
    // ESTO NOS PERMITE SEGUIR UNO DE LOS 5 PRINCIPIOS SOLID:
    // Principio de Inversion de Dependencias: Las clases dependen de abstracciones(Interfaces) y no de tipos concretos(Ej. Clases).
    // - Puede haber un problema si la dependencia Ej. ServicioA tiene otra dependencia se la tendriamos que pasar, pero que pasa 
    //   si la dependencia de ServicioA tiene a su vez otra dependencia, seria una locura.
    // ESTO SE SOLUCIONA USANDO EL SISTEMA DE INYECCION DE DEPENDENCIAS DE ASP .NETCORE.

    //public AutoresController(ApplicationDbContext db, IServicio servicio) // RECORDEMOS QUE ISERVICIO NO SE ESTA INYECTANDO!.
    //{
    //    _db = db;
    //    _servicio = servicio;
    //}

    //public void Ejemplo()
    //{
    //    var autorController = new AutoresController(null, new ServicioA(new Logger()));// LO ESTAMOS HACIENDO MANUALMENTE LA INSTANCIACION.
    //    var autorController2 = new AutoresController(null, new ServicioB());
    //}

    #endregion

    #endregion

    #region Sistema de INYECCION DE DEPENDENCIAS
    // Para poder utilizar el sistema de inyeccion debemos de ir a la clase STARTUP
    // STARTUP: Se configuran los servicios -> metodo ConfigureServices y el Pipeline o Middlewares -> metodo Configure()
    // Servicios: Un servicio no es mas que la resolucion de una dependencia configurada en el sistema de inyeccion.
    // Con el sistema de inyeccion se centraliza y suple automaticamente las depedencias de todas nuestras clases.

    // EJEMPLO:
    // services.AddDbContext<ApplicationDbContext>(options =>
    //  {
    //    options.UseSqlServer(Configuration.GetConnectionString("DbConnectionString"));
    //  });
    // El addDbContext se encarga de configurar el ApplicatonDbContext como un servicio, esto quiere decir que cada vez que el 
    // ApplicatonDbContext aparezca como una dependencia de una clase a travez del constructor. El sistema de inyeccion se encargara
    // de instanciar correctamene el ApplicationDbContext con todas sus configuraciones. 
    // El addDbContext configura por defecto el servicio como Scoped.
    // Se pueden configurar servicios basados en CLASES y INTERFACES.

    // EJEMPLO CON INTERFAZ:
    // services.AddTransient<IServicio, ServicioA>(); -> Cuando una clase requiera un IServicio pasale una instancia de la clase ServicioA.
    // Recordemos que ServicioA tiene dependencia con ILogger, pero, ASP.net ya trae configurado el ILogger como servicio.

    // EJEMPLO TIPO CONCRETO(Clase)
    // services.AddTransient<ServicioA>();

    //TIPOS DE SERVICIOS
    // Singleton: Siempre tendremos la misma instancia, sin importar la peticion http o el usuario. Siempre se comparte la misma instancia.
    //  - Si tenemos una capa de cache en memoria donde se tiene datos uniformes para servirla de forma rapida a los usuarios.
    // Scoped: Dentro del mismo contexto(peticion) http se nos dara la misma instancia.
    //  - Es util cuando por ejemplo implementas un patron UnitOfWork donde en distintas partes de la app realizando cambios en el dbcontext.
    // Transient: Se dara siempre una nueva instancia no importa que sea en el mismo contexto(peticion) http.
    //  - Es bueno para simples funciones que ejecuten funcionalidad, sin tener que mantener datos que se reutilizará en otros lugares.

    #endregion

    #region Logger
    // Podemos procesar mensajes en nuestra app y colocar dichos mensajes en algun lugar(DB, consola, archivo de texto)
    // Como tipo generico se le pasa la clase donde te encuentras, es para saber de donde se esta generando el msg de log.
    //public AutoresController(ApplicationDbContext db, ILogger<AutoresController> logger)
    //{
    //    _db = db;
    //}
    // -Por defecto tenemos un proveedor configurado que escribe los mensajes en la consola
    // - Un proveedor nos permite indicar que ocurrira con los mensajes que estamos mandando. Determina que va a pasar con los mensajes.
    // - Se pueden tener varios proveedores simultaneamene con el objetivo de mandar los mensajes a distintos lugares al mismo tiempo.
    // - Es una configuracion centralizada que se replicara en toda la APP.

    // Tipos de mensajes de LOG (menor serveridad a mayor) - 
    // 1. Trace
    // 2. Debug
    // 3. Information
    // 4. Warning
    // 5. Error
    // 6. Critical
    // No siempre se va querer procesar absolutamente todos los mensajes ya que pueden haber muchos mensajes de information, debug, trace
    // en toda la app.
    // Se puede configurar cuando se quiere que realmente se procesen categorias de mensajes.
    // Dicha configuracion se encuentra en el Appsettings -> Appsettings.developmente -> LogLevel.
    // La configuracion se puede hacer por namespaces.
    #endregion

    #region Middlewares
    //Una peticion http llega a la web api y pasa por una tuberia de peticiones HTTP.    
    //Tuberia: Es una cadena de procesos conectados de tal forma que la salida de cada elemento
    //de la cadena es la entrada del proximo.
    // --Ejemplo de middleware.
    // En el proceso de los ENDPOINTS donde se maneja hacia donde van las peticiones HTTP al ser recibidas, en este
    // caso se enviaran dichas peticiones a los respectivos controladores segun la ruta utilizada por el cliente de la app.
    // A cada proceso se le llama middlewares.
    // El orden de los middlewares es importante.
    #endregion

    #region Filtros
    // Nos ayudan a correr codigo en determinados momentos del ciclo de vida del procesamiento de una peticion HTTP
    // En lo que respecta al middleware relacionado con nuestros controladores, lo que antes era el middleware MVC.
    //Son uiles cuando se tiene la necesidad de ejecutar una logica en varias acciones de varios controladores y queremos
    // evitar que repetir codigo
    //Tipos de filtros
    // Filtro de autenticacion: Bloquear acceso a usuarios sin autenticacion
    // Filtro de Recursos: Se ejecutan despues de la etapa de autorizacion. Pueden detener la tuberia de filtros
    // Filtros de accion: Se ejecutan justo antes o despues de la ejecucion de una accion. Se pueden usar para manipular
    // los argumentos enviados a una accion o la informacion retornada por los mismos.
    // Filtros de excepcion: Se ejecutan cuando hubo una excepcion no atrapada en un try catch durante la ejecucion de una accion.
    // Filtro de resultado: Se ejecutan antes y despues de un action result.

    //Alcance de filros: A nivel de accion, A nivel de controlador, A nivel global.
    #endregion

    [Route("api/[controller]")] // ruta del controlador -> [controller] es como un placeholder que en tiempo de ejecucion se sustutiria
    [ApiController]             // por el nombre del controlador
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly ILogger<AutoresController> _logger;

        public IMapper _mapper { get; }

        public AutoresController(ApplicationDbContext db, ILogger<AutoresController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        // El endpoint o accion puede tener varias rutas para poder ser accedido
        [HttpGet] // ruta heredada del route -> api/autores
        [HttpGet("listado")] // nueva ruta -> api/autores/listado
        [HttpGet("/listado")] // sustituye a la ruta del route por una nueva ruta -> /listado -> midominio.com/listado
        [ResponseCache(Duration = 10)] // La respuesta se guardara en memoria. Se usa para ahorrar tiempo de procesamiento.
        [ServiceFilter(typeof(Mifiltrodeaccion))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await _db.Autores.ToListAsync();
            return _mapper.Map<List<AutorDTO>>(autores);
        }

        // Parametros de ruta: {nombrevariable:tipo de dato} - al no usar la restriccion 
        // de tipo de dato se obtendrá un 400 - bad request
        /*
         [HttpGet("{id:int}/{parametro?}")] -> parametro opcional que tendrá valor null
         [HttpGet("{id:int}/{parametro=persona}")] -> parametro opcional con valor por defecto
         */
        [HttpGet("{id:int}", Name ="ObtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id){
            var autor = await _db.Autores
                .Include(autorDB=> autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB=> autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);

            if (autor == null) return NotFound();

            return _mapper.Map<AutorDTOConLibros>(autor);
        }    

        [HttpGet("{nombre}")]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> Get(string nombre)
        {
            var autores = await _db.Autores.Where(x => x.Nombre.Contains("nombre")).ToListAsync();
        
            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AutorCreacionDTO autor)
        {
            try
            {
                var autorExiste = await _db.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

                if (autorExiste)
                {
                    return BadRequest($"Ya existe un autor con nombre {autor.Nombre}");
                }

                var autorMappeado = _mapper.Map<Autor>(autor);

                _db.Add(autorMappeado);
                await _db.SaveChangesAsync();
                var autorDTO = _mapper.Map<AutorDTO>(autor);

                return CreatedAtRoute("ObtenerAutor", new {id=autorMappeado.Id}, autorDTO);
            }
            catch (Exception e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
        } 

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(AutorCreacionDTO autorDTO, int id)
        {
            var autor = _mapper.Map<Autor>(autorDTO);
            autor.Id = id;

            _db.Update(autor);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _db.Autores.AnyAsync(autor => autor.Id == id);

            if (!result)
            {
                return NotFound();
            }

            _db.Remove(new Autor() { Id = id});
            await _db.SaveChangesAsync();
            return Ok();
        }

    }
}
