using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic
{
    public interface IFactory
        : IDisposable
    {
        bool Enabled { get; }

        bool Initialized { get; }

        void Initialize();
    }

    public interface IFactory<TProvider, TActor>
        : IFactory
        where TProvider : class
        where TActor : class
    {
        IEnumerable<TProvider> Providers { get; }

        TActor Create<TCategory>() where TCategory : class;
    }
}
