using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task  InsertarParametroPaginacionEnRespuesta<T>(
                this HttpContext context, IQueryable<T> queryable, int cantidadRegistrosAMostrar
            )
        {
            if(context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            double conteo = await queryable.CountAsync();
            double TotalPaginas = Math.Ceiling(conteo / cantidadRegistrosAMostrar);
            context.Response.Headers.Add("conteo", conteo.ToString());
            context.Response.Headers.Add("totalPaginas", TotalPaginas.ToString());
        }
    }
}
