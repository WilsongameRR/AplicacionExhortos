namespace AplicacionExhortos.Models
{
    public class DetalleExhortoModel
    {
        public consultaExhortos Exhorto { get; set; } = new consultaExhortos();

        public List<DiligenciaModel> Diligencias { get; set; } = new();
    }
}