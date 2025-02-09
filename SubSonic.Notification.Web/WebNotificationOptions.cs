using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Azure.SignalR;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public class WebNotificationOptions
        : SubSonicOptions
    {
        public WebNotificationOptions() 
        {
            Endpoint = "notification";
        }

        public WebNotificationOptions(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; set; }
        /// <summary>
        /// true, use a redis backplane to scale signalr servers
        /// </summary>
        /// <remarks>the configuration can use only one solution for scalability</remarks>
        public bool UseStackExchangeRedis => _options.ContainsKey(nameof(RedisOptions)) && UseAzureServices == false;
        /// <summary>
        /// true, use azure cloud services to scale signalr servers
        /// </summary>
        /// <remarks>the configuration can use only one solution for scalability</remarks>
        public bool UseAzureServices => _options.ContainsKey(nameof(ServiceOptions)) && UseStackExchangeRedis == false;
    }
}
