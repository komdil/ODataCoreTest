using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace ODataCoreTest
{
    /// <summary>
    /// Authentication response
    /// </summary>
    public class EagleODataAuthenticationResponse
    {
        /// <summary>
        /// Returns true if header of request contains Token
        /// </summary>
        public bool IsTokenBasedAuthentication { get; private set; }

        /// <summary>
        /// Token from header of request
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Basic authorization UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Basic authorization Password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Returns true if authentication was successfully completed
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// Authentification rejected error message
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Returns false if header of request does not contains authentification
        /// </summary>
        public bool IsAuthenticationHeaderValid { get; private set; }

        /// <summary>
        /// Reading authentification credientials from header of request
        /// </summary>
        public static EagleODataAuthenticationResponse GetFromHeader(IHeaderDictionary header)
        {
            var authenticationHeader = new EagleODataAuthenticationResponse();

            if (header.TryGetValue("Token", out StringValues value))
            {
                authenticationHeader.IsTokenBasedAuthentication = true;
                authenticationHeader.Token = value;
                authenticationHeader.IsAuthenticationHeaderValid = true;
                return authenticationHeader;
            }
            else
            {
                if (!header.ContainsKey("Authorization"))
                {
                    authenticationHeader.ErrorMessage = "Authorization headers missed!";
                    return authenticationHeader;
                }

                var authHeader = AuthenticationHeaderValue.Parse(header["Authorization"]);
                string[] credentials;
                try
                {
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                }
                catch
                {
                    credentials = authHeader.Parameter.Split(new[] { ':' }, 2);
                }
                authenticationHeader.UserName = credentials[0];
                authenticationHeader.Password = credentials.Length < 2 ? null : credentials[1];
                authenticationHeader.IsAuthenticationHeaderValid = true;
                return authenticationHeader;
            }
        }
    }
}
