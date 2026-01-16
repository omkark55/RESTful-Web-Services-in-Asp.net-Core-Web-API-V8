using Microsoft.AspNetCore.SignalR;

namespace WebApi_Angular.Models
{
    public class MainHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
