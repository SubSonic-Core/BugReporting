using Microsoft.Extensions.Options;
using SubSonic.Configuration.Options;
using System;

namespace SubSonic.Configuration
{
    public class SubSonicEnvironment
    {
        public const string DevUnstable = $"{nameof(DevUnstable)}";
        public const string Dev = $"{nameof(Dev)}";
        public const string Test = $"{nameof(Test)}";
        public const string Prod = $"{nameof(Prod)}";

        private readonly IOptions<EnvironmentOptions> _options;

        public SubSonicEnvironment(IOptions<EnvironmentOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public EnvironmentOptions Options => _options.Value;

        public static implicit operator string(SubSonicEnvironment env) => env.Options.Environment;
    }
}
