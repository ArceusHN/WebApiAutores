using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middlewares
{

    public static class LogResponsesHTTPExtensions
    {
        public static IApplicationBuilder UseLogResponsesHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogResponsesHTTP>();
        }
    }

    public class LogResponsesHTTP
    {
        private readonly RequestDelegate next;// A travez de este se indican los siguientes middlewares de la tuberia.

        public LogResponsesHTTP(RequestDelegate next, ILogger<LogResponsesHTTP> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public ILogger<LogResponsesHTTP> logger { get; }

        //Invoke o InvokeAsync -> retornar un task y aceptar un httpcontext
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var OriginalBody = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string answer = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(OriginalBody);
                context.Response.Body = OriginalBody;

                logger.LogInformation(answer);
            }
        }
    }
}
