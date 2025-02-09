using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SubSonic.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubSonic.Security
{
    public class SubSonicSecurityOptions
        : SubSonicOptions
    {
        public string? Authority { get; set; }
        /// <summary>
        /// Set SymmetricSecurityKey as a byte array
        /// </summary>
        public byte[] SymmetricSecurityKey { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// In Seconds, the time span that the tokens are considered valid, default is 0 seconds
        /// </summary>
        public TimeSpan TokensExpireInSeconds { get; set; } = TimeSpan.FromSeconds(0);
        /// <summary>
        /// Get a issuer that will be used to validate the issuer of the token, default is null
        /// </summary>
        public string? Issuer {  get; set; }
        /// <summary>
        /// Get a issuer that will be used to validate the audience of the token, default is null
        /// </summary>
        public string? Audience {  get; set; }

        public bool RequireHttpsMetadata { get; set; } = true;

        private const string access_token = "access_token";

        internal void AddJwtBearer(JwtBearerOptions options)
        {
            options.Authority = Authority;

            options.RequireHttpsMetadata = RequireHttpsMetadata;

            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = (ctx) =>
                {
                    ctx.Token = ((string?)ctx.Request.Headers.Authorization)?.Replace("Bearer", string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(ctx.Token) && ctx.Request.Headers.ContainsKey(SubSonicSecurityHeaders.EncodedUserJwt))
                    {
                        ctx.Token = ctx.Request.Headers[SubSonicSecurityHeaders.EncodedUserJwt][0];
                    }
                    if (ctx.Request.Path.StartsWithSegments("/hub") &&
                        ctx.Request.Query.ContainsKey(access_token) &&
                        string.IsNullOrWhiteSpace(ctx.Token))
                    {   // possible signalr hub request
                        ctx.Token = ctx.Request.Query[access_token];
                    }
                    return Task.CompletedTask;
                }
            };

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(SymmetricSecurityKey)
            };
        }
    }
}
