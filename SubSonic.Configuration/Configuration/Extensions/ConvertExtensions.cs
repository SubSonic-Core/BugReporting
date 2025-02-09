using Microsoft.Extensions.Configuration;
using System;

namespace SubSonic.Configuration
{
    public static class ConvertExtensions
    {
        public static TType GetValue<TType>(this string value)
        {
            return (TType)Convert.ChangeType(value, typeof(TType));
        }

        public static bool ToBoolean(this string value)
        {
            return value.GetValue<bool>();
        }

        public static int ToInt(this string value)
        {
            return value.GetValue<int>();
        }

        public static long ToInt64(this string value)
        {
            return value.GetValue<long>();
        }

        public static TType GetRequiredValue<TType>(this IConfigurationSection section, string key)
        {
            if (string.IsNullOrWhiteSpace(section[key]))
            {
                throw new ArgumentNullException(key, $"{key} missing from section, {section.Path}");
            }

            return GetValue<TType>(section[key]!);
        }
    }
}
