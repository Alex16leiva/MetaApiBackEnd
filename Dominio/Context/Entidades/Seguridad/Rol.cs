using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.Seguridad
{
    public class Rol : Entity
    {
        [ForeignKey("Usuario")]
        public required string RolId { get; set; }
        public required string Descripcion { get; set; }
        public virtual List<Permisos>? Permisos { get; set; }
        public virtual ICollection<Usuario>? Usuarios { get; set;}

    }
}

