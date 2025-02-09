using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SubSonic.Testing;

public static class Extensions
{
    public static TChainTarget If<TChainTarget>(this TChainTarget chainTarget, bool value, Action<TChainTarget> block)
    {
        if (value)
        {
            block(chainTarget);
        }

        return chainTarget;
    }

    public static bool IsTestIntegration(this IHostEnvironment env)
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals(Settings.AspNetCoreEnvironment) ?? false;
    }

    public static bool IsMiddlewareAdded<T>(this IApplicationBuilder app) => app.IsMiddlewareAdded(typeof(T).Name);

    public static bool IsMiddlewareAdded(this IApplicationBuilder app, string typeName)
    {
        if (app.Properties.TryGetValue("__MiddlewareDescriptions", out var value) &&
            value is IList<string> descriptions)
        {
            return descriptions.Any(x => x.EndsWith(typeName));
        }

        return false;
    }

    public static DateTime ConvertFromUnixTimestamp(this double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp);
    }

    public static double ConvertToUnixTimestamp(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }

}


