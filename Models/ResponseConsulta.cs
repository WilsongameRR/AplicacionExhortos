namespace AplicacionExhortos.Models
{
    public class ResponseConsulta
    {
        public List<consultaExhortos> ConsultaExhortos { get; set; } = new();
    }

    public class consultaExhortos
    {
        public int ExhortoId { get; set; }

        public string? idOrigen { get; set; }

        public string? tuaOrigen { get; set; }

        public string? NoExhortoEnviado { get; set; }

        public string? NoExpediente { get; set; }

        public string? NoOficio { get; set; }

        public string? Estado { get; set; }

        public string? Municipio { get; set; }

        public string? Poblado { get; set; }

        public string? idDestino { get; set; }

        public string? tuaDestino { get; set; }

        public string? FechaAcuerdo { get; set; }

        public string? FechaAudiencia { get; set; }

        public string? FechaEnvio { get; set; }

        public string? Estatus { get; set; }
    }
}