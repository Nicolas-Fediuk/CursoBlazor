using System.Timers;
using Timer = System.Timers.Timer;

namespace BlazorPeliculas.Client.Auth
{
    public class RenovardorToken : IDisposable
    {
        private readonly ILoginService loginService;

        public RenovardorToken(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        Timer? timer;

        public void Iniciar()
        {
            //me permite ejkercutar una funcion periodicamente
            timer = new Timer();
            timer.Interval = 1000 * 60 * 4; // 4 minutos, tiene que ser menor a los 5 min
            //timer.Interval = 1000 * 5; // 5 seg
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            loginService.ManejarRenovacionToken();
        }

        public void Dispose()
        {
            // Limpiar el timer
            timer?.Dispose();
        }
    }
}
