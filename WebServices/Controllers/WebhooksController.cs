using Aplicacion.DTOs.Mensaje;
using Aplicacion.Services.whatsapp;
using Dominio.Context.Entidades.Mensaje;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebServices.Controllers.models;

namespace WebServices.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WhatsappAppService _whatsappService;
        //private readonly IHubContext<MessageHub> _hubContext;

        public WebhooksController(IConfiguration configuration,
                                    WhatsappAppService whatsappService)
        {
            _configuration = configuration;
            _whatsappService = whatsappService;
        }

        [HttpGet]
        public IActionResult VerifyWebhook()
        {
            var hubMode = Request.Query["hub.mode"].ToString();
            var hubChallenge = Request.Query["hub.challenge"].ToString();
            var hubVerifyToken = Request.Query["hub.verify_token"].ToString();

            var verifyToken = _configuration["MetaSettings:VerifyToken"];

            if (hubMode == "subscribe" && hubVerifyToken == verifyToken)
            {
                return Ok(hubChallenge);  // Meta necesita recibir esto como respuesta
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveMessage([FromBody] MetaMessageDto body)
        {
            try
            {

                string sender = body.Entry.First().Changes.First().Value.Contacts.First().Profile.Name;
                string text = body.Entry.First().Changes.First().Value.Messages.First().Text.Body;
                string phoneNumber = body.Entry.First().Changes.First().Value.Messages.First().From;



                var nuevoMensajeDetalle = new MensajeDetalleDTO
                {
                    NumeroTelefono = phoneNumber,
                    TipoMensaje = "Recibido",
                    Texto = text,
                };

                var newMessage = new MensajeEncabezadoDTO
                {
                    NumeroTelefono = phoneNumber,
                    Nombre = sender,
                    //Platform = "Whatsapp",
                    MensajeDetalle = [nuevoMensajeDetalle],
                };

                var request = new MensajeRequest
                {
                    MensajeEncabezado = newMessage,
                    RequestUserInfo = new Aplicacion.DTOs.RequestUserInfo
                    {
                        UsuarioId = "MetaApi"
                    }
                    
                };

                
                _whatsappService.CrearMensaje(request);

                // Enviar el mensaje a todos los clientes conectados
                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", sender, text, phoneNumber);

                return Ok(); // Meta necesita esta respuesta
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
