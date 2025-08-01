using BlazorPeliculas.Shared.Entidades;
using Microsoft.JSInterop;

namespace BlazorPeliculas.Client.Helpers
{
    public static class IJSRutimeExtensionMethods
    {
        public static async ValueTask<bool> Confirm(this IJSRuntime js, string mensaje)
        {
            // funciones de js de tipo void
            //await js.InvokeVoidAsync("console.log", "Prueba de console log");
            return await js.InvokeAsync<bool>("confirm", mensaje);
        }
    }
}
