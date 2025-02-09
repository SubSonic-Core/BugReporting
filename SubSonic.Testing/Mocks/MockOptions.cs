using Microsoft.Extensions.Options;

namespace SubSonic.Testing.Mocks
{
    internal class MockOptions<TOptions>
        : IOptions<TOptions>
        where TOptions : class
    {
        public MockOptions(TOptions value) { 
            Value = value;
        }

        public TOptions Value { get; }
    }
}
