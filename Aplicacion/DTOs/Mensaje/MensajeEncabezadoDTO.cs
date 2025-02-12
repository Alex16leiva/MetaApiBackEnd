namespace Aplicacion.DTOs.Mensaje
{
    public class MensajeEncabezadoDTO : ResponseBase
    {
        public string? NumeroTelefono { get; set; }
        public string? Nombre { get; set; }
        public string? UltimoMensajeRecibido { get; set; }
        public DateTime FechaUltimoMensajeRecibido { get; set; }
        public List<MensajeDetalleDTO>? MensajeDetalle { get; set; }
    }
}
