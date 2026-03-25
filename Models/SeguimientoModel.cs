using System.ComponentModel.DataAnnotations;

namespace AplicacionExhortos.Models
{
    public class SeguimientoModel
    {
        public int ExhortoId { get; set; }

        [Required(ErrorMessage = "La fecha de recepción es obligatoria.")]
        public DateTime? FechaRecepcion { get; set; }

        [Required(ErrorMessage = "El número de folio es obligatorio.")]
        public string? NoFolio { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        public string? Estatus { get; set; }

        [Required(ErrorMessage = "La fecha de acuerdo TUA exhortado es obligatoria.")]
        public DateTime? FechaAcuerdoTuaExhortado { get; set; }

        [Required(ErrorMessage = "La fecha turno actuaría es obligatoria.")]
        public DateTime? FechaTurnoActuaria { get; set; }

        public string? Observaciones { get; set; }

        public DateTime? FechaAudiencia { get; set; }
        public DateTime? FechaDevolucion { get; set; }
    }
}