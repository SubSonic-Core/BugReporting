using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SubSonic
{
    public static partial class Extensions
    {
        public static IEnumerable<ValueTuple<string, object>> ToEnumerable(this ITuple tuple)
        {
            Func<ITuple, (string, object)> getValueTuple = (ITuple _tuple) =>
            {
                if (_tuple.Length != 2)
                {
                    throw new InvalidOperationException("tuple argument length is invalid.");
                }

                if (_tuple[0] == null)
                {
                    throw new ArgumentNullException($"Item1 in tuple is null.");
                }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string item1 = _tuple[0].ToString()!;
                object item2 = _tuple[1]!;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return new ValueTuple<string, object>(item1, item2);
            };

            Type type = tuple.GetType();

            var genericTypeArg = type.GenericTypeArguments.FirstOrDefault();

            if (genericTypeArg != null &&
                typeof(ValueType).IsAssignableFrom(genericTypeArg))
            {
                foreach (var field in type.GetFields())
                {
                    var value = field.GetValue(tuple);

                    if (value is ITuple _tuple)
                    {
                        yield return getValueTuple(_tuple);
                    }
                }
            }
            else
            {
                yield return getValueTuple(tuple);
            }
        }
    }
}
