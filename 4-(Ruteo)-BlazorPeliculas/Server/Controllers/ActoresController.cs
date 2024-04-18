using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/actores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "personas";

        public ActoresController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper) 
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        //url?pagina=1&CantidadRegistro=5, para obtener los mismos campos de la url que la del modelo dto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametroPaginacionEnRespuesta(queryable, paginacionDTO.CantidadRegistros);
            return await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();    
        }

        //api/actores/buscar/tom
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Actor>>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Actor>();
            }

            textoBusqueda = textoBusqueda.ToLower();

            return await context.Actores.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(5).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Actor>> Get(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            return actor;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Actor actor)
        {
            if(!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actor.Foto = await almacenadorArchivos.GuardarArchivo(fotoActor, ".jpg", contenedor);
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Actor actor)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == actor.Id);

            if(actorDB is null)
            {
                return NotFound();
            }

            // copias los datos de actor y los pega en actorDB
            actorDB = mapper.Map(actor,actorDB);

            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actorDB.Foto = await almacenadorArchivos.EditarArchivo(fotoActor, ".jpg", contenedor, actorDB.Foto!);
            }
            
            // actualiza los datos con los datos de actorDB
            await context.SaveChangesAsync(); 

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            //se elimina asi para tambien eliminar la foto
            var actor = await context.Actores.FirstOrDefaultAsync(x =>x.Id == id);  

            if (actor is null) 
            {
                return NotFound();
            }

            context.Remove(actor);  
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(actor.Foto!, contenedor);

            return NoContent();
        }
    }
}
