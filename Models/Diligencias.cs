namespace AplicacionExhortos.Models
{
    public class Diligencias

    {
        public int TipoDiligenciaId { get; set; }
        public string TipoDiligenciaDesc { get; set; } = string.Empty;

        public string NoExhorto { get; set; } = string.Empty;

        public string Destinatario { get; set; } = string.Empty;
    }
}