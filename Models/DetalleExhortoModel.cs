namespace AplicacionExhortos.Models
{
    public class DetalleExhortoModel
    {
        public ConsultaExhortos Exhorto { get; set; } = new ConsultaExhortos();
        public List<DiligenciaModel> Diligencias { get; set; } = new List<DiligenciaModel>();
        public SeguimientoModel Seguimiento { get; set; } = new SeguimientoModel();
    }
}