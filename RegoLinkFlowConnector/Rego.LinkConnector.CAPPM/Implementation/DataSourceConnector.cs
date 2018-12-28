using ITROI.Clarity.XogClient;
using Rego.LinkConnector.CAPPM.Authentication.Implementation;
using Rego.LinkConnector.CAPPM.Resources;
using Rego.LinkConnector.Core.Actions.DTO;
using Rego.LinkConnector.Core.Authentication.DTO;
using Rego.LinkConnector.Core.Implementation;
using Rego.LinkConnector.Core.Resources.Contracts;
using Rego.LinkConnector.Core.Resources.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.CAPPM.Implementation
{
    /// <summary>
    /// CA PPM datasource implementation
    /// </summary>
    public class DataSourceConnector : IDataSourceConnector
    {
        /// <summary>
        /// XOGAuthentication instance
        /// </summary>
        private XOGAuthentication _xogAuthentication;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataSourceConnector()
        {
            this._xogAuthentication = new XOGAuthentication();
        }

        #region IDataSourceConnector Implementation
        /// <summary>
        /// Gets the authentication parameters from a request headers
        /// </summary>
        /// <param name="headers">HTTP request headers</param>
        /// <returns>DTO with authentication parameters</returns>
        public AuthenticationParametersDTO GetAuthenticationParametersFromRequestHeaders(HttpRequestHeaders headers)
        {
            return this._xogAuthentication.GetAuthenticationParametersFromRequestHeaders(headers);
        }

        /// <summary>
        /// Applies authentication in the endpoint
        /// </summary>
        /// <param name="authenticationParametersDTO">DTO with authentication parameters</param>
        /// <returns>HTTP authentication response</returns>
        public HttpResponseMessage EndPointAuthenticate(AuthenticationParametersDTO authenticationParametersDTO)
        {
            return this._xogAuthentication.EndPointAuthenticate(authenticationParametersDTO);
        }

        /// <summary>
        /// Closes open session
        /// </summary>
        public void EndPointLogOut()
        {
            this._xogAuthentication.EndPointLogOut();
        }

        /// <summary>
        /// Gets the datasource available actions
        /// </summary>
        /// <returns></returns>
        public IList<ActionDTO> GetActions()
        {
            return this._xogAuthentication.GetActiveActions();
        }
        #endregion
    }
}
