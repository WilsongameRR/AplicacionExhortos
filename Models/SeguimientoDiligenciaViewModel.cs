namespace AplicacionExhortos.Models
{
    public class SeguimientoDiligenciaViewModel
    {
        public int DiligenciaId { get; set; }
        public int ExhortoId { get; set; }
        public int TipoDiligenciaId { get; set; }

        public string? TipoDiligenciaDesc { get; set; }
        public string? OtraEspecificar { get; set; }
        public string? Destinatario { get; set; }
        public DateTime? FechaDiligencia { get; set; }
        public string? EstatusDiligencia { get; set; }
        public DateTime? FechaAudiencia { get; set; }

        // Campos de seguimiento
        public string? Observaciones { get; set; }
        public DateTime? FechaSeguimiento { get; set; }
        public string? Resultado { get; set; }
    }
}