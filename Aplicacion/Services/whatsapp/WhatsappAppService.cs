using Aplicacion.Core;
using Aplicacion.DTOs.Mensaje;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Mensaje;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Services.whatsapp
{
    public class WhatsappAppService : BaseDisposable
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;

        public WhatsappAppService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public List<MensajeDetalleDTO> ObtenerMensajePorNumero(string numeroTelefono)
        {
            var mensajeDetalle = _genericRepository.GetFiltered<MensajeDetalle>(r => r.NumeroTelefono == numeroTelefono);

            if (mensajeDetalle.IsNull())
            {
                return [];
            }

            return mensajeDetalle.Select(r => new MensajeDetalleDTO
            {
                Id = r.Id,
                NumeroTelefono = r.NumeroTelefono,
                Texto = r.Texto,
                TipoMensaje = r.TipoMensaje,
            }).ToList();

            
        }

        public List<MensajeEncabezadoDTO> ObtenerMensajes()
        {
            var includes = new List<string> { "MensajeDetalle" };
            var mensaje = _genericRepository.GetAll<MensajeEncabezado>(includes);

            var result = mensaje.Select(x => new MensajeEncabezadoDTO
            {
                NumeroTelefono = x.NumeroTelefono,
                Nombre = x.Nombre,
                UltimoMensajeRecibido = x.MensajeDetalle.LastOrDefault().Texto,
                FechaUltimoMensajeRecibido = x.FechaUltimoMensajeRecibido,
                MensajeDetalle = x?.MensajeDetalle?.Select(r => new MensajeDetalleDTO
                {
                    Id = r.Id,
                    NumeroTelefono = r.NumeroTelefono,
                    Texto = r.Texto,
                    TipoMensaje = r.TipoMensaje,
                }).ToList()
            }).ToList();

            return result.IsNotNull() 
                ? [.. result.OrderByDescending(x => x.FechaUltimoMensajeRecibido)]
                : [];
        }

      

        public MensajeEncabezadoDTO CrearMensaje(MensajeRequest request)
        {
            var numero = _genericRepository.GetSingle<MensajeEncabezado>(r => r.NumeroTelefono == request.MensajeEncabezado.NumeroTelefono);

            var tipomensaje = request?.MensajeEncabezado?.MensajeDetalle?.FirstOrDefault()?.TipoMensaje?.ValueOrEmpty();

            var mensajeDetalleNuevo = new MensajeDetalle
            {
                NumeroTelefono = request.MensajeEncabezado.NumeroTelefono,
                Texto = request.MensajeEncabezado.MensajeDetalle.First().Texto,
                TipoMensaje = tipomensaje,
            };

            if (numero.IsNull())
            {

                var mensajeNuevo = new MensajeEncabezado
                {
                    NumeroTelefono = request.MensajeEncabezado.NumeroTelefono,
                    Nombre = request.MensajeEncabezado.Nombre,
                    FechaUltimoMensajeRecibido = DateTime.Now,
                    MensajeDetalle = [mensajeDetalleNuevo]
                };

                _genericRepository.Add(mensajeNuevo);
            }
            else
            {
                numero.FechaUltimoMensajeRecibido = DateTime.Now;
                numero.MensajeDetalle = [mensajeDetalleNuevo];
            }
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo($"AgregarMensaje{tipomensaje}");
            _genericRepository.UnitOfWork.Commit(transactionInfo);
            return new MensajeEncabezadoDTO(); 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _genericRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
