using BlazorPeliculas.Shared.Entidades;

namespace BlazorPeliculas.Client.Repositorios
{
    public class Repositorio : IRepositorio
    {
        public List<Pelicula> ObtenerPeliculas()
        {
            return new List<Pelicula>()
            {
                new Pelicula{Titulo = "Hulk", FechaLanzamiento = new DateTime(2008, 01, 01)},
                new Pelicula{Titulo = "Betty", FechaLanzamiento = new DateTime(2006, 02, 01)},
                new Pelicula{Titulo = "Gol", FechaLanzamiento = new DateTime(2001, 03, 01)}
            };
        }
    }
}
