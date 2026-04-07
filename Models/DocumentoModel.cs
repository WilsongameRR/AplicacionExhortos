namespace AplicacionExhortos.Models.Exhortos
{
    public class DocumentoModel
    {
        public string NoExhorto { get; set; } = string.Empty;
        public int TipoDocumentoId { get; set; }
        public string Documento { get; set; } = string.Empty;
    }
}