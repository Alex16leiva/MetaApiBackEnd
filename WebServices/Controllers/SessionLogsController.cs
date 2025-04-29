using Aplicacion.DTOs.Seguridad;
using Aplicacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebServices.Controllers
{
    [Route("api/sesionLog")]
    [ApiController]
    public class SessionLogsController : ControllerBase
    {
        private readonly SesionLogApplicationService _sesionLogApplicationService;

        public SessionLogsController(SesionLogApplicationService sesionLogApplicationService)
        {
            _sesionLogApplicationService = sesionLogApplicationService;
        }

        [Authorize]
        [HttpPost("inicio")]
        public SesionLogDTO RegistrarSesionLog(SesionLogRequest request)
        {
            var response = _sesionLogApplicationService.RegistrarSesionLog(request);
            return response;
        }

        [Authorize]
        [HttpPost("fin")]
        public SesionLogDTO TerminarSesionLog(SesionLogRequest request)
        {
            var response = _sesionLogApplicationService.TerminarSesionLog(request);
            return response;
        }
    }
}
