using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Rego.LinkConnector.Core.Resources.Contracts;
using Rego.LinkConnector.Core.Resources.Implementation;
using System.Net.Http.Headers;
using System.Net;
using ITROI.Clarity.XogClient;
using Rego.LinkConnector.Core.Authentication.DTO;
using Rego.LinkConnector.CAPPM.Resources;
using ITROI.Clarity.XogClient.Exceptions;

namespace Rego.LinkConnector.CAPPM.Authentication.Implementation
{
    /// <summary>
    /// CA PPM XOG authentication implementation
    /// </summary>
    public class XOGAuthentication
    {
        /// <summary>
        /// Core resources implementation
        /// </summary>
        private ICoreResourcesBLL _coreResourcesBLL;

        /// <summary>
        /// CA PPM resoruces reference
        /// </summary>
        private CAPPMResourcesBLL _CAPPMResourcesBLL;

        /// <summary>
        /// IXogClient implementation
        /// </summary>
        private IXogClient _xogClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public XOGAuthentication()
        {
            this._coreResourcesBLL = new CoreResourcesBLL();
            this._CAPPMResourcesBLL = new CAPPMResourcesBLL();
        }

        #region IAuth Implementation
        /// <summary>
        /// Gets the authentication parameters from a request headers
        /// </summary>
        /// <param name="headers">HTTP request headers</param>
        /// <returns>DTO with authentication parameters</returns>
        public AuthenticationParametersDTO GetAuthenticationParametersFromRequestHeaders(HttpRequestHeaders headers)
        {
            try
            {
                if (headers.Authorization == null)
                {
                    return null;
                }

                string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(headers.Authorization.Parameter));

                string[] authenticationParams = decodedAuthenticationToken.Split('|');

                if (authenticationParams.Length != 2)
                {
                    return null;
                }

                string url = authenticationParams[0],
                       usernameAndPassword = authenticationParams[1];

                int index = usernameAndPassword.IndexOf(':');

                string username = usernameAndPassword.Substring(0, index),
                       password = usernameAndPassword.Substring(username.Length + 1);

                if (string.IsNullOrEmpty(url) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password))
                {
                    return null;
                }

                return new AuthenticationParametersDTO
                {
                    URL = url,
                    Username = username,
                    Password = password
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Applies authentication in the endpoint
        /// </summary>
        /// <param name="authenticationParametersDTO">DTO with authentication parameters</param>
        /// <returns>HTTP authentication response</returns>
        public HttpResponseMessage EndPointAuthenticate(AuthenticationParametersDTO authenticationParametersDTO)
        {
            bool unauthorized = false;

            try
            {
                if (authenticationParametersDTO == null)
                {
                    unauthorized = true;
                    throw new Exception(this._coreResourcesBLL.GetResource("ERROR_AUTHENTICATION_FAILED"));
                }

                if (!authenticationParametersDTO.URL.ToLower().Contains("/niku/xog"))
                {
                    unauthorized = true;
                    throw new Exception(this._CAPPMResourcesBLL.GetResource("ERROR_INVALID_CAPPM_URL"));
                }

                this._xogClient = new XogClient(authenticationParametersDTO.URL);
                string sessionID = null;

                try
                {
                    sessionID = this._xogClient.GetSessionId(authenticationParametersDTO.Username,
                                                             authenticationParametersDTO.Password);
                }
                catch (XogClientLoginException ex)
                {
                    unauthorized = true;
                    throw ex;
                }

                if (string.IsNullOrEmpty(sessionID))
                {
                    unauthorized = true;
                    throw new Exception(this._xogClient.GetResponse());
                }

                return new HttpResponseMessage();
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                if (ex.InnerException != null &&
                    !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    message += " " + ex.InnerException.Message;
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent(message),
                    StatusCode = unauthorized ? HttpStatusCode.Unauthorized 
                                              : HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Closes open session
        /// </summary>
        public void EndPointLogOut()
        {
            if (this._xogClient != null)
            {
                this._xogClient.DisposeSession();
            }
        }
        #endregion
    }
}
