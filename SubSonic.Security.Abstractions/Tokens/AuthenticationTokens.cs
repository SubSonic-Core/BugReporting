using SubSonic.Cache;

namespace SubSonic.Security.Tokens
{
    public class AuthenticationTokens        
    {
        public AuthenticationTokens(string accessToken, string userJwt) 
        {
            AccessToken = accessToken;
            EncodedUserJwt = userJwt;
        }

        public string AccessToken { get; }

        public string EncodedUserJwt { get; }
    }
}
