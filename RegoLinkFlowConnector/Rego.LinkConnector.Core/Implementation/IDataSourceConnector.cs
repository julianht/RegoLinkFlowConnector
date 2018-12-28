using Rego.LinkConnector.Core.Actions.DTO;
using Rego.LinkConnector.Core.Authentication.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Implementation
{
    /// <summary>
    /// Contract with datasource public methods
    /// </summary>
    public interface IDataSourceConnector
    {
        /// <summary>
        /// Gets the authentication parameters from a request headers
        /// </summary>
        /// <param name="headers">HTTP request headers</param>
        /// <returns>DTO with authentication parameters</returns>
        AuthenticationParametersDTO GetAuthenticationParametersFromRequestHeaders(HttpRequestHeaders headers);

        /// <summary>
        /// Applies authentication in the endpoint
        /// </summary>
        /// <param name="authenticationParametersDTO">DTO with authentication parameters</param>
        /// <returns>HTTP authentication response</returns>
        HttpResponseMessage EndPointAuthenticate(AuthenticationParametersDTO authenticationParametersDTO);

        /// <summary>
        /// Closes open session
        /// </summary>
        void EndPointLogOut();

        /// <summary>
        /// Gets the datasource available actions
        /// </summary>
        /// <returns></returns>
        IList<ActionDTO> GetActions();
    }
}
