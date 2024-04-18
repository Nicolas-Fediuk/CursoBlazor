using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    //Para los usuarios que tengan un token
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly string contenedor = "pelicula";

        public PeliculasController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper, UserManager<IdentityUser> userManager) 
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        //permite ver a los usuarios anonimos
        [AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 6;

            var peliculasEnCartelera = await context.Peliculas.Where(x => x.EnCartelera).Take(limite).OrderByDescending(x => x.FechaLanzamiento).ToListAsync();

            var fechaActual = DateTime.Today;

            var proximosEstrenos = await context.Peliculas.Where(x => x.FechaLanzamiento > fechaActual).OrderBy(x => x.FechaLanzamiento).Take(5).ToListAsync();

            var resultado = new HomePageDTO
            {
                PeliculasEnCartelera = peliculasEnCartelera,
                ProximosEstrenos = proximosEstrenos
            };

            return resultado;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(pelicula => pelicula.Id == id)
                .Include(pelicula => pelicula.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(pelicula => pelicula.PeliculasActor.OrderBy(pa => pa.Orden))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();

            if(pelicula is null)
            {
                return NotFound();
            }

            // TODO: Sistema de votacion
            var promedioVoto = 0.0;
            var votoUsuario = 0;

            if(await context.VotosPeliculas.AnyAsync(x => x.PeliculaId == id))
            {
                promedioVoto = await context.VotosPeliculas.Where(x => x.PeliculaId == id).AverageAsync(x => x.Voto);

                if (HttpContext.User.Identity!.IsAuthenticated)
                {
                    var usuario = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name!);

                    if (usuario is null)
                    {
                        return BadRequest("Usuario no encontrado");
                    }

                    var usuarioId = usuario.Id;

                    var votoUsuarioDB = await context.VotosPeliculas.FirstOrDefaultAsync(x => x.PeliculaId == id && x.UsuarioId == usuarioId);

                    if (votoUsuarioDB is not null)
                    {
                        votoUsuario = votoUsuarioDB.Voto;
                    }
                }
            }

            var modelo = new PeliculaVisualizarDTO();
            modelo.PromedioVotos = promedioVoto;
            modelo.VotoUsuario = votoUsuario;    
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.GenerosPelicula.Select(gp => gp.Genero!).ToList();
            modelo.Actores = pelicula.PeliculasActor.Select(pa => new Actor
            {
                Nombre = pa.Actor!.Nombre,
                Foto = pa.Actor.Foto,
                Personaje = pa.Personaje,
                Id = pa.ActorId
            }).ToList();

            return modelo;
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Pelicula>>> Get([FromQuery] ParametrosBusquedaPeliculasDTO modelo)
        {
            //los queryable con string de query que voy armando parte por parte
            var peliculaQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(modelo.Titulo))
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.Titulo.Contains(modelo.Titulo)); 
            }

            if (modelo.EnCartelera)
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.EnCartelera);
            }

            if(modelo.Estrenos)
            {
                var hoy = DateTime.Today;
                peliculaQueryable = peliculaQueryable.Where(x => x.FechaLanzamiento >= hoy);
            }

            if(modelo.GeneroId != 0)
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.GenerosPelicula.Select(y => y.GeneroId).Contains(modelo.GeneroId));  
            }

            if(modelo.MasVotadas)
            {
                peliculaQueryable = peliculaQueryable.OrderByDescending(p => p.VotosPeliculas.Average(vp => vp.Voto));
            }

            await HttpContext.InsertarParametroPaginacionEnRespuesta(peliculaQueryable, modelo.CantidadRegistros);

            var peliculas = await peliculaQueryable.Paginar(modelo.paginacionDTO).ToListAsync();

            return peliculas;
        }

        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizacionDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);

            if(peliculaActionResult.Result is NotFoundResult) 
            {
                return NotFound();
            }

            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds = peliculaVisualizarDTO!.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos.Where(x => !generosSeleccionadosIds.Contains(x.Id)).ToListAsync();

            var modelo = new PeliculaActualizacionDTO();
            modelo.Pelicula = peliculaVisualizarDTO.Pelicula;
            modelo.GenerosNoSeleccionados = generosNoSeleccionados;
            modelo.GenerosSeleccionados = peliculaVisualizarDTO.Generos;
            modelo.Actores = peliculaVisualizarDTO.Actores;

            return modelo;
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrEmpty(pelicula.Poster))
            {
                var poster = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(poster, ".jpg", contenedor);
            }

            EscribirOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        private static void EscribirOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActor is not null)
            {
                for (int i = 0; i < pelicula.PeliculasActor.Count; i++)
                {
                    pelicula.PeliculasActor[i].Orden = i + 1;
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            var peliculaDb = await context.Peliculas
                .Include(x => x.GenerosPelicula)
                .Include(x => x.PeliculasActor)
                .FirstOrDefaultAsync(x => x.Id == pelicula.Id);

            if(peliculaDb is null)
            {
                return NotFound(); 
            }

            peliculaDb = mapper.Map(pelicula, peliculaDb);

            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDb.Poster = await almacenadorArchivos.EditarArchivo(posterImagen, ".jpg", contenedor, peliculaDb.Poster!);
            }

            EscribirOrdenActores(peliculaDb);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            //se elimina asi para tambien eliminar la foto
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }

            context.Remove(pelicula);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(pelicula.Poster!, contenedor);

            return NoContent();
        }

    }
}
