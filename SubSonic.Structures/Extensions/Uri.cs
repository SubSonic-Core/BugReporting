using System;

namespace SubSonic
{
    public static partial class Extensions
    {
        public static Uri Append(this Uri uri, string path)
        {
            return new Uri($"{uri.ToString().TrimEnd('/')}/{path}");
        }

        public static Uri AppendIf(this Uri uri, bool @if, string path)
        {
            if (@if)
                return uri.Append(path);
            return uri;
        }
    }
}
