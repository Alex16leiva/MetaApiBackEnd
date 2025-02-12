namespace Aplicacion.DTOs.Mensaje
{
    public class MensajeDetalleDTO : ResponseBase
    {
        public int Id { get; set; }
        public string? NumeroTelefono { get; set; }
        public string? TipoMensaje { get; set; }
        public string? Texto { get; set; }
    }
}
