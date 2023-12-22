using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext context; //Inicializamos como un campo de la clase
        private readonly IMapper mapper;

        public AutoresController(ApplicationDBContext context, IMapper mapper)  //Instanciamos ApplicationDBContext con context
        {
            this.context = context;
            this.mapper = mapper;
        }

        
        [HttpGet] //api/autores
        public async Task<List<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {               //autorDB: Representa un registro de la base de datos
            var autor = await context.Autores.FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            return mapper.Map<AutorDTO>(autor);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autores = await context.Autores.Where(autorBDs => autorBDs.Nombre.Contains(nombre)).ToListAsync();

                return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeConElMismoNombre = await context.Autores.AnyAsync(p=> p.Nombre == autorCreacionDTO.Nombre);

            if (existeConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            //Mapeamos autorCreacionDTO hacia Autor. 
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor); //Se marca como próximo a guardar
            await context.SaveChangesAsync(); //Recién se guardan los cambios de manera async
            return Ok();
        }

        [HttpPut("{id:int}")] //api/autores/1
        public async Task<IActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if(!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = id });  //No se crea Autor. Se instancia un tipo Autor
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
