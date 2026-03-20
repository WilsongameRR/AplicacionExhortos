namespace AplicacionExhortos.Models.Exhortos
{
    public class AltaExhortoModel
    {
        public string? Expediente { get; set; }
        public string? NoOficio { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public string? Poblado { get; set; }

        public int? TuaExhortado { get; set; }

        public DateTime? FechaGeneral { get; set; }
        public DateTime? FechaAudiencia { get; set; }
    }
}