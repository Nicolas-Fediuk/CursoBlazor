using BlazorPeliculas.Client;
using BlazorPeliculas.Client.Auth;
using BlazorPeliculas.Client.Repositorios;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IRepositorio, Repositorio>();

builder.Services.AddSweetAlert2();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<RenovardorToken>();

builder.Services.AddScoped<ProveedorAutenticacionJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(proveedor =>
proveedor.GetRequiredService<ProveedorAutenticacionJWT>());
builder.Services.AddScoped<ILoginService, ProveedorAutenticacionJWT>(proveedor =>
proveedor.GetRequiredService<ProveedorAutenticacionJWT>());

await builder.Build().RunAsync();
