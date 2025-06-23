using HFYStorySorter.WebUI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog.Core;
using Serilog.Events;

namespace HFYStorySorter.Logging
{
    public class SignalRLogSink : ILogEventSink
    {
        private static IHubContext<LogHub>? _hubContext;

        public static void Configure(IHubContext<LogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            if (_hubContext != null)
            {
                _hubContext.Clients.All.SendAsync("ReceiveLog", message);
            }
        }
    }
}
