using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth
{
    public class ProveedorAutenticacionPrueba : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonimo = new ClaimsIdentity();

            var usuarioNico = new ClaimsIdentity(
                //que informacion necesito del usuario autenticado 
                new List<Claim>
                {
                    new Claim("llave1","valor1"),
                    new Claim(ClaimTypes.Name,"Nico"),
                    new Claim(ClaimTypes.Role,"admin")
                }
                , authenticationType: "prueba"); 

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonimo)));
        }
    }
}
