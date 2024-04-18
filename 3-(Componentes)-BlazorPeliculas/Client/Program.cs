using BlazorPeliculas.Client;
using BlazorPeliculas.Client.Repositorios;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//Servicios
builder.Services.AddSingleton<ServicioSingleton>();
builder.Services.AddTransient<ServicioTransient>();

builder.Services.AddSingleton<IRepositorio, Repositorio>();

//otra manera
//ConfigureServices(builder.Services);
//void ConfigureServices(IServiceCollection services)
//{
//    services.AddSingleton<ServicioSingleton>();
//    services.AddTransient<ServicioTransient>();
//}

await builder.Build().RunAsync();
