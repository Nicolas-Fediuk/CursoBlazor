using BlazorPeliculas.Client.Helpers;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.Tracing;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {
        [Inject] ServicioSingleton singleton { get; set; } 
        [Inject] ServicioTransient transient { get; set; }
        [Inject] IJSRuntime js { get; set; }

        //[CascadingParameter(Name = "Color")] protected string Color { get; set; }
        //[CascadingParameter(Name ="Size")] protected string Size { get; set; }

        [CascadingParameter] protected AppState appState { get; set; }

        //asi se usa para almacenar modulos de js
        IJSObjectReference modulo;

        private int currentCount = 0;
        private static int currentCountStatic = 0;

        [JSInvokable]
        public async Task IncrementCount()
        {
            //cuando se invoque este metodo, ahi se va a descargar el archivo js
            modulo = await js.InvokeAsync<IJSObjectReference>("import", "./js/Counter.js");
            //asi invoco la funcion 
            await modulo.InvokeVoidAsync("mostrarAlerta","Hola a todos");


            currentCount++;
            singleton.valor = currentCount;
            transient.valor = currentCount;
            currentCountStatic = currentCount;
            try
            {
                await js.InvokeVoidAsync("a");
            }
            catch(Exception ex)
            {
                string mensaje = ex.Message;    
            }
            
        }

        List<Pelicula> ObtenerPeliculas()
        {
            return new List<Pelicula>()
            {
                new Pelicula{Titulo = "asd", FechaLanzamiento = new DateTime(2008, 01, 01)},
                new Pelicula{Titulo = "Besdftty", FechaLanzamiento = new DateTime(2006, 02, 01)},
                new Pelicula{Titulo = "Gdfgol", FechaLanzamiento = new DateTime(2001, 03, 01)}
            };
        }

        private async Task IncrementCountJs()
        {
            //paso una instancia de la clase Counter
            await js.InvokeVoidAsync("pruebaPuntoNetInstancia",
                DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public static Task<int> ObtenerCurrentCount()
        {
            Console.WriteLine("Entro current");
            return Task.FromResult(currentCountStatic);  
        }
    }
}
