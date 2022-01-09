using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filtros
{
    public class Mifiltrodeaccion : IActionFilter
    {
        private readonly ILogger<Mifiltrodeaccion> logger;

        public Mifiltrodeaccion(ILogger<Mifiltrodeaccion> logger)
        {
            this.logger = logger;   
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Antes de ejecutar la accion");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Despues de ejecutar la accion");
          
        }
    }
}
