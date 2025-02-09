using System;

namespace SubSonic.Configuration
{
    public interface ISubSonicOptions
    {
        bool Enabled { get; }

        object[] GetOptions(params Type[] types);
    }
}
