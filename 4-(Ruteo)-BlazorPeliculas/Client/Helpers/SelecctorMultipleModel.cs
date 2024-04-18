namespace BlazorPeliculas.Client.Helpers
{
    public class SelecctorMultipleModel
    {
        public SelecctorMultipleModel(string llave, string valor)
        {
            Llave = llave;
            Valor = valor;
        }   

        public string Llave { get; set; }
        public string Valor { get; set; }   
    }
}
