namespace SubSonic.Configuration
{
    public abstract class ProviderWithOptions<TOptions>
        : Provider
        where TOptions : class
    {
        private readonly TOptions _options;

        public ProviderWithOptions(TOptions options) {
            _options = options;
        }

        protected TOptions Options => _options;
    }
}
