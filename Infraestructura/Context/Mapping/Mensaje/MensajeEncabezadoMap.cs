using Dominio.Context.Entidades.Mensaje;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Mensaje
{
    internal class MensajeEncabezadoMap : EntityMap<MensajeEncabezado>
    {
        public override void Configure(EntityTypeBuilder<MensajeEncabezado> builder)
        {
            builder.HasKey(x => x.NumeroTelefono);
            builder.ToTable("MensajeEncabezado", "dbo");
            builder.Property(x => x.NumeroTelefono).HasColumnName("NumeroTelefono").IsRequired().HasMaxLength(25);
            builder.Property(x => x.Nombre).HasColumnName("Nombre").IsRequired();
            builder.Property(x => x.FechaUltimoMensajeRecibido).HasColumnName("FechaUltimoMensajeRecibido");
            builder.HasMany(m => m.MensajeDetalle).WithOne(md => md.MensajeEncabezado).HasForeignKey(md => md.NumeroTelefono);


            base.Configure(builder);
        }
    }
}
