namespace AplicacionExhortos.Models
{
    public class ResponseBd
    {
        public int NoError { get; set; }

        public string? Mensaje { get; set; }

        public int IdGenerado { get; set; }

        public string? Valor { get; set; }
    }
}