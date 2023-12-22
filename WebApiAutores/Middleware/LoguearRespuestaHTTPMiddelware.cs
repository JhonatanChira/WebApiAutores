using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middleware
{

    public static class LoguearRespuestaHTTPMiddelwareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddelware>();
        }
    }

    public class LoguearRespuestaHTTPMiddelware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddelware> logger;

        //El RequestDelegate permite invocar los siguientes Middelware de la tubería
        public LoguearRespuestaHTTPMiddelware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddelware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        //Invoke o InvokeAsync
            //Este Middleware se encarga de guardar el cuerpo de la respuesta en ILogger
        public async Task InvokeAsync(HttpContext contexto)
        {
            // Entrada del middleware

            // 1. Creo un MemoryStream para poder manipular y copiarme el cuerpo de la respuesta.
            // Esto se hace porque el stream del cuerpo de la respuesta no tiene permisos de lectura.
            using (var ms = new MemoryStream())
            {
                // 2. Guardo la referencia del Stream donde se escribe el cuerpo de la respuesta
                var cuerpoOriginalRespuesta = contexto.Response.Body;

                // 3. Cambio el stream por defecto del cuerpo de la respuesta por el MemoryStream creado
                // para poder manipularlo
                contexto.Response.Body = ms;


                // 4. Esperamos a que el siguiente middleware devuelva la respuesta.
                await siguiente(contexto);

                // Salida del middleware

                // 5. Nos movemos al principio del MemoryStream para copiar el cuerpo de la respuesta
                ms.Seek(0, SeekOrigin.Begin);

                // 6. Leemos stream hasta el final y almacenamos el cuerpo de la respuesta obtenida
                string respuesta = new StreamReader(ms).ReadToEnd();

                // 5. Nos volvemos a posicionar al principio del MemoryStream para poder copiarlo al 
                // cuerpo original de la respuesta
                ms.Seek(0, SeekOrigin.Begin);


                // 7. Copiamos el contenido del MemoryStream al stream original del cuerpo de la respuesta
                await ms.CopyToAsync(cuerpoOriginalRespuesta);

                // 8.Volvemos asignar el stream original al cuerpo de la respuesta para que siga el flujo normal.
                contexto.Response.Body = cuerpoOriginalRespuesta;

                // 9. Escribimos en el log la respuesta obtenida
                logger.LogInformation(respuesta);
            }
        }
    }
}
