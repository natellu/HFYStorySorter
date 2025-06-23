using Microsoft.AspNetCore.SignalR;

namespace HFYStorySorter.WebUI.Hubs
{
    public class LogHub : Hub
    {
        public async Task SendLogMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveLogMessage", message);
        }

        public async Task SendTestMessage()
        {
            await Clients.Caller.SendAsync("ReceiveLog", "Test message from server at " + DateTime.Now);
        }
    }
}
