namespace WebApiAutores.Servicios
{
    public class Escribirenarchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo1.txt";
        private Timer timer;    

        public Escribirenarchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)// Al cargar el web api
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proceso iniciado...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) // Al apagar el web api
        {
            timer.Dispose();
            Escribir("Proceso finalizado...");
            return Task.CompletedTask;
        }

        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }

        }

        private void DoWork(object state)
        {
            Escribir("Proceso en ejecucion" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

    }
}
