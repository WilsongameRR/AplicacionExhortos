namespace AplicacionExhortos.Models
{
    public class ResponseConsulta
    {
        public List<ConsultaExhortos> ConsultaExhortos { get; set; } = new();
    }

    public class ConsultaExhortos
    {
        public int ExhortoId { get; set; }
        public string? IdOrigen { get; set; }
        public string? TuaOrigen { get; set; }
        public string? NoExhortoEnviado { get; set; }
        public string? NoExhortoRecibido { get; set; }
        public string? NoExpediente { get; set; }
        public string? NoOficio { get; set; }
        public string? Folio { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public string? Poblado { get; set; }
        public string? IdDestino { get; set; }
        public string? TuaDestino { get; set; }
        public string? FechaAcuerdo { get; set; }
        public string? FechaAudiencia { get; set; }
        public string? FechaEnvio { get; set; }
        public string? FechaRecibido { get; set; }
        public string? FechaAcuerdoExhortado { get; set; }
        public string? FechaTurnoActuaria { get; set; }
        public string? FechaDevolucion { get; set; }
        public string? Observaciones { get; set; }
        public string? Estatus { get; set; }

    }
}