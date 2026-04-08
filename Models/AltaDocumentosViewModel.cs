using System.Collections.Generic;

namespace AplicacionExhortos.Models.Exhortos
{
    public class AltaDocumentosViewModel
    {
        public string NoExhorto { get; set; } = string.Empty;

        public List<DocumentoModel> Documentos { get; set; } = new();
    }
}