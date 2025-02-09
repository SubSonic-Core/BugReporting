using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SubSonic.Security
{
    public class SubSonicJwtToken
    {
        private readonly IOptions<SubSonicSecurityOptions> _options;
        private readonly JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();

        public SubSonicJwtToken(IOptions<SubSonicSecurityOptions> options)
        {
            _options = options;
        }

        SubSonicSecurityOptions Options => _options.Value;

        public string GenerateJwtToken(string username, params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Options.SymmetricSecurityKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Options.Issuer,
                audience: Options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(Options.TokensExpireInSeconds),
                signingCredentials: credentials);

            return _jwtHandler.WriteToken(token);
        }
    }
}
