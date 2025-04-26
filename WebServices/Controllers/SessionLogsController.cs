using Aplicacion.DTOs.Seguridad;
using Aplicacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionLogsController : ControllerBase
    {
        private readonly SesionLogApplicationService _sesionLogApplicationService;

        public SessionLogsController(SesionLogApplicationService sesionLogApplicationService)
        {
            _sesionLogApplicationService = sesionLogApplicationService;
        }

        [Authorize]
        [HttpPost("log")]
        public SesionLogDTO RegistrarSesionLog(SesionLogRequest request)
        {
            var response = _sesionLogApplicationService.RegistrarSesionLog(request);
            return response;
        }

    }
}
