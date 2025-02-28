using Aplicacion.DTOs.Mensaje;
using Aplicacion.Services.whatsapp;
using Dominio.Context.Entidades.Mensaje;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebServices.Controllers.models;

namespace WebServices.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WhatsappAppService _whatsappService;
        private readonly IHubContext<ChatHub> _hubContext;

        public WebhooksController(IConfiguration configuration,
                                    WhatsappAppService whatsappService,
                                    IHubContext<ChatHub> hubContext)
        {
            _configuration = configuration;
            _whatsappService = whatsappService;
            _hubContext = hubContext;
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
                Message message = body.Entry.First().Changes.First().Value.Messages.First();
                string text = message.Text.Body;
                string phoneNumber = message.From;

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

                await ProcesarMensaje(message);

                return Ok(); // Meta necesita esta respuesta
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return BadRequest();
            }
        }

        
        private async Task ProcesarMensaje(Message message)
        {
            string fileUrl = message.Type == "image" || message.Type == "video"
                ? await ObtenerArchivoDesdeMeta(message.Id)
                : string.Empty;

            await _hubContext.Clients.All.SendAsync(
                "ReceiveMessage", message.From, message.Text.Body, fileUrl, message.Type);
        }


        private async Task<string> ObtenerArchivoDesdeMeta(string mediaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TU_ACCESS_TOKEN");
                var response = await client.GetAsync($"https://graph.facebook.com/v19.0/{mediaId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var mediaData = JsonConvert.DeserializeObject<MetaMediaResponse>(json);
                    return mediaData.Url;
                }
            }
            return string.Empty;
        }
    }
}
