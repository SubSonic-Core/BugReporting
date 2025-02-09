using Microsoft.Extensions.DependencyInjection;
using System;
using Option = Microsoft.Extensions.Options.Options;

namespace SubSonic.Configuration
{
    public abstract class PackageBuilder<TOptions>
        : PackageBuilder
        , IBuilder<TOptions>
        where TOptions : class, ISubSonicOptions, new()
    {
        protected readonly TOptions _options;

        public PackageBuilder(string name)
            : base(name) { 
            _options = new TOptions();
        }

        public TOptions Options => _options;

        public abstract IBuilder Configure(Action<TOptions> options);

        public virtual void SetConfiguration(TOptions options)
        {
            foreach(var property in typeof(TOptions).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(options, property.GetValue(_options));
                }
            }
        }
    }

    public abstract class PackageBuilder
        : IBuilder
    {
        private readonly string _name;

        public PackageBuilder(string name)
        {
            _name = name ?? Option.DefaultName;
        }

        public string Name => _name;

        public abstract bool Enabled { get; }

        public virtual IServiceCollection Services => throw new NotImplementedException("override this property in derived class.");
    }
}
