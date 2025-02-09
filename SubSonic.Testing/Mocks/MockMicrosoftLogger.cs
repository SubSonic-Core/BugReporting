using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubSonic.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SubSonic.Testing.Mocks
{
    public abstract class MockMicrosoftLogger<TCategory>
        : ILogger<TCategory>
    {
        private readonly StateBag _stateBag = new StateBag();
        private readonly IOptions<LogOptions> _options;

        public MockMicrosoftLogger(IOptions<LogOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public LogOptions Options => _options.Value;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (state is IEnumerable<KeyValuePair<string, object>> collection)
            {
                _stateBag.AddCollection(collection);
            }
            else if (state is ITuple tuple)
            {
                foreach ((string, object) data in tuple.ToEnumerable())
                {
                    _stateBag.TryAdd(data.Item1, data.Item2);
                }
            }

            return _stateBag;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Options.MinLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, Func<TState, System.Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Log(logLevel, eventId, formatter.Invoke(state, exception));
            }
        }

        public abstract void Log(LogLevel logLevel, EventId eventId, string message);
    }
}
