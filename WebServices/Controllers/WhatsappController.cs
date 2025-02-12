using Aplicacion.DTOs.Mensaje;
using Aplicacion.Services.whatsapp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServices.Controllers
{
    [Route("api/whatsapp")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsappAppService _whatsappAppService;
        public WhatsappController(WhatsappAppService whatsappAppService)
        {
            _whatsappAppService = whatsappAppService;
        }

        [AllowAnonymous]
        [HttpPost("enviar-mensaje")]
        public IActionResult EnviarMensaje([FromBody] MensajeRequest request)
        {
            var resultado = _whatsappAppService.CrearMensaje(request);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("crete-mensaje-recibido")]
        public IActionResult CrearMensajeRecibido([FromBody] MensajeRequest request) 
        {
            var resultado = _whatsappAppService.CrearMensaje(request);
            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("obtener-mensajes")]
        public IActionResult ObtenerMensajes()
        {
            var resultado = _whatsappAppService.ObtenerMensajes();

            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpGet("obtener-mensaje-por-numero")]
        public IActionResult ObternerMensajePorNumeroTelefono([FromQuery] string numeroTelefono)
        {
            var response = _whatsappAppService.ObtenerMensajePorNumero(numeroTelefono);
            return Ok(response);
        }
    }
}
