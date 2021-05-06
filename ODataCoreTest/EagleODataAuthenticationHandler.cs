using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    /// <summary>
    /// Handling authentication
    /// </summary>
    public class EagleODataAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        readonly EagleODataAuthenticationService userService;
        string BodyResponse;

        /// <summary/>
        public EagleODataAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, EagleODataAuthenticationService userService)
            : base(options, logger, encoder, clock)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Used to ensure HandleAuthenticateAsync is only invoked once. The subsequent calls
        /// will return the same authenticate result.
        /// </summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            EagleODataAuthenticationResponse authenticationResponse = await userService.Authenticate(Request);
            authenticationResponse.UserName = "admin";
            if (!authenticationResponse.Success)
            {
                BodyResponse = authenticationResponse.ErrorMessage;
                return AuthenticateResult.Fail(authenticationResponse.ErrorMessage);
            }
            else
            {
                IEnumerable<Claim> claims;
                if (authenticationResponse.IsTokenBasedAuthentication)
                {
                    claims = new[] { new Claim(ClaimTypes.Name, authenticationResponse.Token), };
                }
                else
                {
                    claims = new[] { new Claim(ClaimTypes.Name, authenticationResponse.UserName), };
                }
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }

        /// <summary>
        /// Writing authentication error on response
        /// </summary>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (BodyResponse != null && BodyResponse != "")
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(BodyResponse);
                return Task.Run(new Action(() =>
                {
                    Response.StatusCode = 401;
                    Response.Body.Write(byteArray);
                }));
            }
            return base.HandleChallengeAsync(properties);
        }
    }
}
