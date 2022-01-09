using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filtros
{
    public class Filtrodeexcepcion:ExceptionFilterAttribute
    {
        private readonly ILogger<Filtrodeexcepcion> logger;

        public Filtrodeexcepcion(ILogger<Filtrodeexcepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
