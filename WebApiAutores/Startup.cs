using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middleware;

namespace WebApiAutores
{                   //ESTE ES NUESTRO SISTEMA DE INYECIÓN DE DEPENDENCIAS (STARTUP)
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }  //Propiedad

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones=>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions(x=>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

                //Usamos AddDbContext para configurar el servicio del ApplicationDBContext
            services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
                //Indicamos que usamos SqlServer y luego se pasa el GetConnectionString.

            //Para un sistema de autenticación.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
            });

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHTTP();
            

           
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1"));
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
