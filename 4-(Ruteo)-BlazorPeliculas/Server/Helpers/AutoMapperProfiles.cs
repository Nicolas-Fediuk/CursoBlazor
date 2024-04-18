using AutoMapper;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;

namespace BlazorPeliculas.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {

        // para que el automapper no tome el campo de Foto, que lo ignore
        public AutoMapperProfiles()
        {
            CreateMap<Actor, Actor>().ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Pelicula, Pelicula>().ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<VotoPeliculaDTO, VotoPelicula>();
        }
    }
}
