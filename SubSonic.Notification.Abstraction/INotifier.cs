namespace SubSonic.Notification
{
    public interface INotifier
    {
        bool Enabled { get; }

        /// <summary>
        /// send a message to all users
        /// </summary>
        /// <param name="message"></param>
        Task BroadCastAsync(string message);
        /// <summary>
        /// send a message to a list of one or more users
        /// </summary>
        /// <param name="message"></param>
        /// <param name="users"><see cref="string[]"/></param>
        /// <returns></returns>
        Task SendAsync(string message, params string[] users);
    }
}
