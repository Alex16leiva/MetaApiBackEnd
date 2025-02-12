using Dominio.Core;
using System.ComponentModel.DataAnnotations;

namespace Dominio.Context.Entidades.Mensaje
{
    public class MensajeEncabezado : Entity
    {
        [Key]
        public string? NumeroTelefono { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaUltimoMensajeRecibido { get; set; }
        public virtual List<MensajeDetalle>? MensajeDetalle { get; set; }
    }
}
