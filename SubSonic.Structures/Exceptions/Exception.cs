using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic
{
    public class Exception
        : System.Exception
    {
        public Exception()
            : base() { }

        public Exception(string? message)
            : base(message) { }

        public Exception(string? message, System.Exception? inner)
            : base(message, inner) { }

        protected TType? GetExceptionData<TType>(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key)); 
            }

            if (Data[key] is TType data)
            {
                return data;
            }

            return default;
        }

        protected internal void SetExceptionData(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Data[key] = value;
        }
    }
}
