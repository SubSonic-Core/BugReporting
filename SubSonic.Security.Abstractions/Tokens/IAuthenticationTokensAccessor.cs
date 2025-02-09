using System.Security.Claims;
using System.Threading.Tasks;

namespace SubSonic.Security.Tokens
{
    public interface IAuthenticationTokensAccessor
    {
        /// <summary>
        /// Retrieve the Authentication Tokens for the spcecified user
        /// </summary>
        /// <returns><see cref="AuthenticationTokens"></returns>
        Task<AuthenticationTokens> GetAuthenticationTokensAsync(string username, params Claim[] claims);
    }
}
