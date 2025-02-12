using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades.Mensaje
{
    public class MensajeDetalle : Entity
    {
        public int Id { get; set; }
        [ForeignKey("MensajeEncabezado")]
        public string NumeroTelefono { get; set; } = string.Empty;
        public string? TipoMensaje { get; set; }
        public string? Texto { get; set; }
        public virtual MensajeEncabezado? MensajeEncabezado { get; set; }
    }
}
