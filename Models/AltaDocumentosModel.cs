using System.Text.Json;

namespace AplicacionExhortos.Models.Exhortos
{
    public class AltaDocumentosModel
    {
        public string NoExhorto { get; set; } = string.Empty;

        public string DocumentosJson { get; set; } = string.Empty;

        public List<DocumentoModel> ObtenerDocumentos()
        {
            if (string.IsNullOrWhiteSpace(DocumentosJson))
                return new List<DocumentoModel>();

            return JsonSerializer.Deserialize<List<DocumentoModel>>(DocumentosJson)
                   ?? new List<DocumentoModel>();
        }
    }
}