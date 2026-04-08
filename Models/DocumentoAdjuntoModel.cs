namespace AplicacionExhortos.Models.Exhortos
{
    public class DocumentoAdjuntoModel
    {
        public int ExhortoId { get; set; }
        public int DocumentoId { get; set; }
        public int TipoDocumentoId { get; set; }

        public string TipoDocumentoDesc { get; set; } = string.Empty;
        public string DocumentoAlfresco { get; set; } = string.Empty;
    }
}