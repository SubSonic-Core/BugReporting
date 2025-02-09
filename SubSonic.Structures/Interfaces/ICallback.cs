using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic
{
    public interface ICallback
    {
        Action Callback { get; }
    }
}
