using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/generos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class GeneroController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public GeneroController(ApplicationDbContext context) 
        {
            this.context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Genero>>> Get()
        {
            return await context.Generos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if(genero is null) 
            { 
                return NotFound();
            }

            return genero;
        }

        [HttpPost]
        //IActionResult<int> para que retorne un entero
        public async Task<ActionResult<int>> Post(Genero genero)
        {
            // para agregar un nuevo genero
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
            //ok pero sin retornar nada
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var filasAfectadas = await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();

            if(filasAfectadas == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
