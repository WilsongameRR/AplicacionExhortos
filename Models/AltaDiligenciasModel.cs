using System.Collections.Generic;

namespace AplicacionExhortos.Models
{
    public class AltaDiligenciasModel
    {
        public string? NoExhorto { get; set; }

        public List<DiligenciaModel> Diligencias { get; set; } = new List<DiligenciaModel>();
    }
}