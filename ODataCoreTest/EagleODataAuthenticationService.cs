using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ODataCoreTest
{
    /// <summary>
    /// Service for authenticating to OData Web Api
    /// </summary>
    public class EagleODataAuthenticationService
    {
        #region Sessions

        /// <summary>
        /// Authenticate using token or basic authentication
        /// </summary>
        static EagleODataAuthenticationResponse AuthentificateAndGetSession(HttpRequest request)
        {
            var eagleODataAuthenticationResponse = EagleODataAuthenticationResponse.GetFromHeader(request.Headers);
            eagleODataAuthenticationResponse.Success = true;
            return eagleODataAuthenticationResponse;
        }


        #endregion

        /// <summary>
        /// Authenticate using token or basic authentication
        /// </summary>
        public async Task<EagleODataAuthenticationResponse> Authenticate(HttpRequest request)
        {
            return await Task.Run(() => AuthentificateAndGetSession(request));
        }
    }
}
