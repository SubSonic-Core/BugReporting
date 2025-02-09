using Microsoft.Extensions.Logging;

namespace SubSonic.Logging
{
    public class LogOptions
        : LogDefineOptions
    {
        public LogLevel MinLevel { get; set; }
    }
}
