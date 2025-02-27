﻿using Dominio.Context.Entidades.Mensaje;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Infraestructura.Context.Mapping.Mensaje;
using Infraestructura.Context.Mapping.Seguridad;
using Infraestructura.Core;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Context
{
    public class AppDbContext : BCUnitOfWork, IDataContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> context)
            : base(context)
        {
            Database.SetCommandTimeout((int)TimeSpan.FromSeconds(1).TotalSeconds);
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Rol> Rol {  get; set; }
        public virtual DbSet<MensajeEncabezado> MensajeEncabezado { get; set; }
        public virtual DbSet<MensajeDetalle> MensajeDetalle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new RolMap());  
            modelBuilder.ApplyConfiguration(new MensajeEncabezadoMap());
            modelBuilder.ApplyConfiguration(new MensajeDetalleMap());
            base.OnModelCreating(modelBuilder);
        }


        public override void Commit(TransactionInfo transactionInfo)
        {
            base.Commit(transactionInfo);
        }
    }
}
