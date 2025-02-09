namespace SubSonic.Notification
{
    public class Notifier
        : INotifier
    {
        private readonly IEnumerable<INotificationChannel> _channels;
        private readonly NotificationOptions _options;

        public Notifier(IEnumerable<INotificationChannel> channels, NotificationOptions options)
        {
            _channels = channels;
            _options = options;
        }

        public bool Enabled => _options.Enabled && _channels.Any();

        public NotifierOptions Options => _options.GetOptions<NotifierOptions>();

        public Task BroadCastAsync(string message)
        {
            if (!Enabled) return Task.CompletedTask;

            var tasks = new List<Task>();

            foreach (var channel in _channels)
            {
                tasks.Add(channel.BroadCastAsync(message));
            }

            return Task.WhenAll(tasks);
        }

        public Task SendAsync(string message, params string[] users)
        {
            if (!Enabled) return Task.CompletedTask;

            var tasks = new List<Task>();

            foreach (var channel in _channels)
            {
                tasks.Add(channel.SendAsync(message, users));
            }

            return Task.WhenAll(tasks);
        }
    }
}
