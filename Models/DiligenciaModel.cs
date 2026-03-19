namespace AplicacionExhortos.Models
{
    public class DiligenciaModel
    {
        public int ExhortoId { get; set; }
        public int DiligenciaId { get; set; }
        public int TipoDiligenciaId { get; set; }

        public string? TipoDiligenciaDesc { get; set; }
        public string? OtraEspecificar { get; set; }
        public string? Destinatario { get; set; }
        public DateTime? FechaDiligencia { get; set; }
        public string? EstatusDiligencia { get; set; }
        public DateTime? FechaAudiencia { get; set; }
    }
}