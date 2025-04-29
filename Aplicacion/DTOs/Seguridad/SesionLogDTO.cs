namespace Aplicacion.DTOs.Seguridad
{
    public class SesionLogDTO : ResponseBase
    {
        public string? UsuarioId { get; set; }
        public string? SesionUser { get; set; }
        public DateTime InicioSesion { get; set; }
        public  DateTime CierreSesion { get; set; }
        public int TiempoActivoSegundos { get; set; }
    }
}
