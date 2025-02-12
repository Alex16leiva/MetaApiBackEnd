using Dominio.Context.Entidades.Mensaje;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Mensaje
{
    internal class MensajeDetalleMap : EntityMap<MensajeDetalle>
    {
        public override void Configure(EntityTypeBuilder<MensajeDetalle> builder)
        {
            builder.ToTable("MensajeDetalle", "dbo");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id");
            builder.Property(x => x.NumeroTelefono).HasColumnName("NumeroTelefono");
            builder.Property(x => x.Texto).HasColumnName("Texto");
            builder.Property(x => x.TipoMensaje).HasColumnName("TipoMensaje");
            base.Configure(builder);
        }
    }
}
