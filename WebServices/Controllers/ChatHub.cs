using Microsoft.AspNetCore.SignalR;

namespace WebServices.Controllers
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string? fileUrl = null, string? fileType = null)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, fileUrl, fileType);
        }
    }
}
