namespace SubSonic.Notification
{
    [Flags]
    public enum NotifierIntentEnum
    {
        None = 0,
        SignalR = 1,
        Email = 2,
        Teams = 4,
        Mobile = 8,
        All = SignalR | Email | Teams | Mobile,
    }
}