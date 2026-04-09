using System.ComponentModel.DataAnnotations;
using AplicacionExhortos.Models.Exhortos;

namespace AplicacionExhortos.Models
{
    public class ReiterarExhortoModel
    {
        public int ExhortoId { get; set; }

        public string? NoExhortoEnviado { get; set; }
        public string? TuaOrigen { get; set; }
        public string? TuaDestino { get; set; }
        public string? NoExpediente { get; set; }
        public string? NoOficio { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public string? Poblado { get; set; }
        public string? Estatus { get; set; }

        [Required(ErrorMessage = "La fecha nuevo acuerdo es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime? FechaNuevoAcuerdo { get; set; }

        [Required(ErrorMessage = "La fecha nueva audiencia es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime? FechaNuevaAudiencia { get; set; }

        public int NumeroEnvios { get; set; }

        public List<DiligenciaModel> Diligencias { get; set; } = new();
        public List<DocumentoAdjuntoModel> DocumentosAdjuntos { get; set; } = new();
    }
}