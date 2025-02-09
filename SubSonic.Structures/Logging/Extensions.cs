using System;
using System.Diagnostics;
using System.Reflection;

namespace SubSonic.Logging
{
    public static class Extensions
    {
        public static string? GetDisplayName(this Type type)
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

            return attribute?.Name;
        }
    }
}
