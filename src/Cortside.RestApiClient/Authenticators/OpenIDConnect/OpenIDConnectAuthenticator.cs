using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Cortside.RestApiClient.Authenticators.OpenIDConnect {
    public class OpenIDConnectAuthenticator : RestApiAuthenticator {
        private readonly TokenRequest tokenRequest;
        private IAsyncPolicy<RestResponse> policy = Policy.NoOpAsync<RestResponse>();
        private ILogger logger = new NullLogger<OpenIDConnectAuthenticator>();
        private DateTime tokenExpiration = DateTime.UtcNow;
        private readonly IHttpContextAccessor context;
        private bool clientAllowsDelegation = false;

        public OpenIDConnectAuthenticator(IHttpContextAccessor context, TokenRequest tokenRequest) : base("") {
            this.context = context;
            this.tokenRequest = tokenRequest;
        }

        public OpenIDConnectAuthenticator(IHttpContextAccessor context, string authorityUrl, string grantType, string clientId, string clientSecret, string scope) : base("") {
            this.context = context;
            tokenRequest = new TokenRequest {
                AuthorityUrl = authorityUrl,
                GrantType = grantType,
                Scope = scope,
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        public OpenIDConnectAuthenticator UsePolicy(IAsyncPolicy<RestResponse> policy) {
            this.policy = policy;
            return this;
        }

        public OpenIDConnectAuthenticator UseLogger(ILogger logger) {
            this.logger = logger;
            return this;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            // will only show first 20 characters of header/token for security
            logger.LogTrace("GetAuthenticationParameter called with token {accessToken}. Token has cached value of {token} with expiration of {expiration}", accessToken.Left(20), Token.Left(20), tokenExpiration);

            // save working token so that another thread does not set it from underneath us
            var currentToken = Token;

            // get a new token if the cached one is not set OR it's expired
            if (string.IsNullOrEmpty(Token) || tokenExpiration <= DateTime.UtcNow) {
                var response = await GetTokenAsync(tokenRequest.AuthorityUrl, tokenRequest.GrantType, tokenRequest.ClientId, tokenRequest.ClientSecret, tokenRequest.Scope).ConfigureAwait(false);
                if (!response.IsSuccessful || response.Data == null) {
                    throw new AuthenticationException(response.ErrorMessage);
                }

                // set both current working token and cached token with expiration date
                currentToken = response.Data.Token;
                Token = currentToken;
                tokenExpiration = response.Data.ExpirationDate;
                clientAllowsDelegation = AllowsDelegation(currentToken);

                if (clientAllowsDelegation) {
                    logger.LogInformation($"Token for client_id {tokenRequest.ClientId} allows delegation");
                }
            }

            // check to see if delegation is allowed and if there is an Authorization header from HttpContext
            if (clientAllowsDelegation) {
                var authorization = context?.HttpContext?.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(authorization)) {
                    logger.LogInformation($"Delegation is allowed and Authorization header was found in HttpContext with value of {authorization.Left(20)}");
                    authorization = authorization.Replace("Bearer ", "");
                    var response = await GetTokenAsync(tokenRequest.AuthorityUrl, "delegation", tokenRequest.ClientId, tokenRequest.ClientSecret, tokenRequest.Scope, authorization).ConfigureAwait(false);
                    if (!response.IsSuccessful || response.Data == null) {
                        logger.LogWarning("Delegation token request failed, falling back to client token");
                    }

                    logger.LogInformation("Delegation token request was successful, using delegation token instead of client token");
                    currentToken = $"{response!.Data!.TokenType} {response!.Data!.AccessToken}";
                }
            }

            logger.LogTrace("Using Authorization header with value of {header}", currentToken.Left(20));
            return new HeaderParameter(KnownHeaders.Authorization, currentToken);
        }

        private bool AllowsDelegation(string token) {
            if (string.IsNullOrWhiteSpace(token)) {
                return false;
            }

            try {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token.Replace("Bearer ", ""));
                var delegation = jwtToken.Claims.Any(x => x.Type == "grant_type" && x.Value == "delegation");
                // check to see if token is for the client to be used for 
                //var self = jwtToken.Claims.Any(x => x.Type == "client_id" && x.Value == tokenRequest.ClientId);

                //return delegation && !self;
                return delegation;
            } catch (Exception ex) {
                logger.LogDebug(ex, "Unable to read token as JWT token to figure out if token has grant_type claim for delegation");
                return false;
            }
        }

        private async Task<RestResponse<TokenResponse>> GetTokenAsync(string authorityUrl, string grantType, string clientId, string clientSecret, string scope, string token = null) {
            var options = new RestClientOptions(authorityUrl);

            using (var client = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson())) {
                var request = new RestRequest("connect/token", Method.Post)
                    .AddParameter("grant_type", grantType)
                    .AddParameter("scope", scope)
                    .AddParameter("client_id", clientId)
                    .AddParameter("client_secret", clientSecret)
                    .AddParameter("token", token ?? string.Empty);

                var correlationId = CorrelationContext.GetCorrelationId();
                request.AddHeader("Request-Id", correlationId);
                request.AddHeader("X-Correlation-Id", correlationId);

                logger.LogInformation($"Getting {grantType} token for client_id {clientId} with scopes [{scope}]");
                var response = await policy.ExecuteAsync(async () => {
                    var response = await client.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);
                    logger.LogInformation($"Identity Server response code: {response?.StatusCode}");
                    return response;
                }).ConfigureAwait(false);

                if (response.IsSuccessful) {
                    // TODO: handling of deserialization exception?
                    // https://github.com/restsharp/RestSharp/blob/5830af48cf85b8eaadf89d83fbc3bf46106f5873/src/RestSharp/Serializers/DeseralizationException.cs
                    var rr = client.Deserialize<TokenResponse>(response);
                    logger.LogDebug($"Successfully obtained {grantType} token for client_id {clientId} with scopes [{scope}]");

                    return rr;
                } else {
                    logger.LogError(response.ErrorException, $"Failed to obtain {grantType} token for client_id {clientId} with scopes [{scope}], Identity Server responded with status: {response.StatusCode} and error {response.ErrorMessage}");
                    return RestResponse<TokenResponse>.FromResponse(response);
                }
            }
        }

        public override void HandleUnauthorizedClientRequest() {
            Token = null;
            tokenExpiration = DateTime.MinValue;
        }
    }
}
