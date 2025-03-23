using Dominio.Core;

namespace Dominio.Context.Entidades.Seguridad
{
    public class Rol : Entity
    {
        public required string RolId { get; set; }
        public required string Descripcion { get; set; }
        public virtual List<Permisos>? Permisos { get; set; }
        public virtual List<Usuario>? Usuarios { get; set;}

    }
}

