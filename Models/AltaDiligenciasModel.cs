using System;
using System.Collections.Generic;

namespace AplicacionExhortos.Models
{
    public class AltaDiligenciasModel
    {
        public string? NoExhorto { get; set; }

        public int TipoDiligenciaId { get; set; }

        public string Otro { get; set; }

        public string? Destinatario { get; set; }

        public DateTime? FechaAudiencia { get; set; }
              
        public string? Especificar { get; set; }

        public List<AltaDiligenciasModel> Diligencias { get; set; } = new List<AltaDiligenciasModel>();
    }
}