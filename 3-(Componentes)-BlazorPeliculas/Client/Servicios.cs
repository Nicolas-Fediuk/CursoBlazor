//esto se va a ejecutar dentro del navegador del usuario

namespace BlazorPeliculas.Client
{
    //Singleton: se va a mantener vijente durante la sesion del usuario,
    //dura toda una pestaña, si se cambia de pestaña, este cambia
    //no importa en que componente o pagina este, este no cambia 
    public class ServicioSingleton
    {
        public int valor { get; set; }
    }

    //Transient: para cada servicio el valor cambia
    //sobrevive en la sesin del componente 
    public class ServicioTransient
    {
        public int valor { get; set; }
    }
}
