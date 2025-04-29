using Dominio.Context.Entidades.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Seguridad
{
    internal class SesionLogMap : EntityMap<SesionLog>
    {
        public override void Configure(EntityTypeBuilder<SesionLog> builder)
        {
            builder.HasKey(r => r.SesionId);
            builder.ToTable("SesionLog", "Seguridad");
            builder.Property(r => r.SesionId).HasColumnName("SesionId");
            builder.Property(r => r.SesionUser).HasColumnName("SesionUser");
            builder.Property(r => r.UsuarioId).HasColumnName("UsuarioId");
            builder.Property(r => r.InicioSesion).HasColumnName("InicioSesion");
            builder.Property(r => r.CierreSesion).HasColumnName("CierreSesion");
            builder.Property(r => r.TiempoActivoSegundos).HasColumnName("TiempoActivoSegundos");
            base.Configure(builder);
        }
    }
}
