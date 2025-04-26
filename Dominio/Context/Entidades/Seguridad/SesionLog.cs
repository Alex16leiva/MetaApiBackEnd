using Dominio.Core;

namespace Dominio.Context.Entidades.Seguridad
{
    public class SesionLog : Entity
    {
        public int SesionId { get; set; }
        public required string UsuarioId { get; set; }
        public required DateTime InicioSesion { get; set; }
        public required DateTime CierreSesion { get; set; }
        public int TiempoActivoSegundos { get; set; }
    }
}
