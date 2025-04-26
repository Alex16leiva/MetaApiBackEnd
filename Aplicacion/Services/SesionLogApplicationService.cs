using Aplicacion.DTOs.Seguridad;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Infraestructura.Context;

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
            var nuevaSesionLog = new SesionLog
            {
                UsuarioId = request.RequestUserInfo.UsuarioId,
                CierreSesion = request.SesionLog.CierreSesion,
                InicioSesion = request.SesionLog.InicioSesion,
                TiempoActivoSegundos = request.SesionLog.TiempoActivoSegundos
            };

            _genericRepository.Add(nuevaSesionLog);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("RegistrarSesionLog");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.SesionLog;
        }
    }
}
