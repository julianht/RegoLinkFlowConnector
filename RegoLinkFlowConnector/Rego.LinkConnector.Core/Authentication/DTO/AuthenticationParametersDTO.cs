using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Authentication.DTO
{
    /// <summary>
    ///DTO with authentication parameters
    /// </summary>
    public class AuthenticationParametersDTO
    {
        /// <summary>
        /// Get/set the authentication URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// /// Get/set the authentication username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Get/set the authentication password
        /// </summary>
        public string Password { get; set; }
    }
}
