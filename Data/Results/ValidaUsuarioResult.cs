namespace AplicacionExhortos.Data.Results
{
    public class ValidaUsuarioResult
    {
        public string? Password { get; set; }
        public string? Nombre { get; set; }
        public int TuaId { get; set; }
        public string? NumTua { get; set; }
        public int ErrorNum { get; set; }
        public string? Mensaje { get; set; }
    }
}
