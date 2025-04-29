using Aplicacion.DTOs.Seguridad;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Infraestructura.Context;
using System;

namespace Aplicacion.Services
{
    public class SesionLogApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        public SesionLogApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public SesionLogDTO RegistrarSesionLog(SesionLogRequest request)
        {
            DateTime fecha = DateTime.Now; 
            string sesionUser = $"{fecha}-{Guid.NewGuid()}";
            
            var nuevaSesionLog = new SesionLog
            {
                UsuarioId = request.RequestUserInfo.UsuarioId,
                SesionUser = sesionUser,
                CierreSesion = fecha,
                InicioSesion = fecha,
                TiempoActivoSegundos = 0
            };

            _genericRepository.Add(nuevaSesionLog);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("RegistrarSesionLog");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            request.SesionLog = new SesionLogDTO { SesionUser = sesionUser };
            return request.SesionLog;
        }

        public SesionLogDTO TerminarSesionLog(SesionLogRequest request)
        {
            var sesionLog = _genericRepository.GetSingle<SesionLog>(r => r.SesionUser == request.SesionLog.SesionUser);

            if (sesionLog == null) {
                return new SesionLogDTO { };
            }

            sesionLog.CierreSesion = DateTime.UtcNow;
            sesionLog.TiempoActivoSegundos = (int)(sesionLog.CierreSesion - sesionLog.InicioSesion).TotalSeconds;
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("TerminarSesionLog");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return new SesionLogDTO();
        }
    }
}
