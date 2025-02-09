using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SubSonic.Security;

namespace SubSonic.Notification
{
    [Authorize(AuthenticationSchemes = SubSonicBearerDefaults.AuthenticationScheme)]
    public class NotificationHub
        : Hub
    {
        public class Methods
        {
            public const string RecieveMessage = nameof(RecieveMessage);
            public const string RecieveSystemMessage = nameof(RecieveSystemMessage);
        }

        public async override Task OnConnectedAsync()
        {
            await Clients.All.SendAsync(Methods.RecieveSystemMessage, $"{(!(Context.User?.Identity?.IsAuthenticated ?? false) ? "Anonymous" : Context.UserIdentifier)} Connected");
        }
    }
}
