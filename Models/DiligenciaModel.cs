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
        public string? FechaDiligencia { get; set; }
        public string? EstatusDiligencia { get; set; }
        public string? FechaAudiencia { get; set; }
    }
}