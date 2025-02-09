using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SubSonic.Cache;
using SubSonic.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SubSonic.Security
{
    public class CredentialManager
        : Tokens.IAuthenticationTokensAccessor
    {
        private readonly ILogger _logger;
        private readonly IExpiringCache<string, Tokens.AuthenticationTokens> _cache;
        private readonly IOptions<SubSonicSecurityOptions> _security;
        private readonly SubSonicEnvironment _environment;
        private readonly SubSonicJwtToken _subSonicJwtToken;

        public CredentialManager(
            ILogger<CredentialManager> logger, 
            IExpiringCache<string, Tokens.AuthenticationTokens> cache,
            IOptions<SubSonicSecurityOptions> security,
            SubSonicEnvironment environment)
        {
            _logger = logger;
            _cache = cache;
            _security = security;
            _environment = environment;
            _subSonicJwtToken = new SubSonicJwtToken(security);
        }

        SubSonicSecurityOptions Security => _security.Value;

        public Task<Tokens.AuthenticationTokens> GetAuthenticationTokensAsync(string username, params Claim[] claims)
        {
            _logger.LogDebug("Get access tokens for {User}", username);

            claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ConvertToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ConvertToUnixTimestamp().ToString())
            }
            .Union(claims).ToArray();

            return Task.FromResult(_cache.Get(username, (user) => new Tokens.AuthenticationTokens("", _subSonicJwtToken.GenerateJwtToken(user, claims))));
        }
    }
}
