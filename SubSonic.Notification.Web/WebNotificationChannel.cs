
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SubSonic.Notification
{
    using HubMethods = NotificationHub.Methods;

    public class WebNotificationChannel
        : INotificationChannel
    {
        private readonly ILogger _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public WebNotificationChannel(ILogger<WebNotificationChannel> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public NotifierProviderIntentEnum Intent => NotifierProviderIntentEnum.SignalR;

        public Task BroadCastAsync(string message)
        {
            return _hubContext.Clients.All.SendAsync(HubMethods.RecieveMessage, message);
        }

        public Task SendAsync(string message, params string[] users)
        {
            return _hubContext.Clients.Users(users).SendAsync(HubMethods.RecieveMessage, message);
        }
    }
}
