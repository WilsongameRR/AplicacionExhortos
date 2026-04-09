using System.ComponentModel.DataAnnotations;

namespace AplicacionExhortos.Models
{
    public class SeguimientoModel : IValidatableObject
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

        public DateTime? FechaVencimiento { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public DateTime? FechaAudiencia { get; set; }

        public string? Observaciones { get; set; }

        // Fecha de referencia para validar cuando NO exista fecha de audiencia
        public DateTime? FechaActualizacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime fechaReferencia = (FechaActualizacion ?? DateTime.Today).Date;

            // Fecha Acuerdo TUA Exhortado
            if (FechaAcuerdoTuaExhortado.HasValue && FechaRecepcion.HasValue)
            {
                if (FechaAcuerdoTuaExhortado.Value.Date < FechaRecepcion.Value.Date)
                {
                    yield return new ValidationResult(
                        "La fecha de acuerdo TUA exhortado debe ser mayor o igual a la fecha de recepción del exhorto.",
                        new[] { nameof(FechaAcuerdoTuaExhortado) });
                }

                if (FechaAudiencia.HasValue)
                {
                    if (FechaAcuerdoTuaExhortado.Value.Date >= FechaAudiencia.Value.Date)
                    {
                        yield return new ValidationResult(
                            "La fecha de acuerdo TUA exhortado debe ser menor a la fecha de audiencia.",
                            new[] { nameof(FechaAcuerdoTuaExhortado) });
                    }
                }
                else
                {
                    if (FechaAcuerdoTuaExhortado.Value.Date > fechaReferencia)
                    {
                        yield return new ValidationResult(
                            "La fecha de acuerdo TUA exhortado debe ser menor o igual a la fecha de actualización del exhorto.",
                            new[] { nameof(FechaAcuerdoTuaExhortado) });
                    }
                }
            }

            // Fecha Turno Actuaría
            if (FechaTurnoActuaria.HasValue && FechaAcuerdoTuaExhortado.HasValue)
            {
                if (FechaTurnoActuaria.Value.Date < FechaAcuerdoTuaExhortado.Value.Date)
                {
                    yield return new ValidationResult(
                        "La fecha turno actuaría debe ser mayor o igual a la fecha del acuerdo TUA exhortado.",
                        new[] { nameof(FechaTurnoActuaria) });
                }

                if (FechaAudiencia.HasValue)
                {
                    if (FechaTurnoActuaria.Value.Date >= FechaAudiencia.Value.Date)
                    {
                        yield return new ValidationResult(
                            "La fecha turno actuaría debe ser menor a la fecha de audiencia.",
                            new[] { nameof(FechaTurnoActuaria) });
                    }
                }
                else
                {
                    if (FechaTurnoActuaria.Value.Date > fechaReferencia)
                    {
                        yield return new ValidationResult(
                            "La fecha turno actuaría debe ser menor o igual a la fecha de actualización del exhorto.",
                            new[] { nameof(FechaTurnoActuaria) });
                    }
                }
            }
        }
    }
}