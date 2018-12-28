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
using Rego.LinkConnector.Core.Log.Contracts;
using Rego.LinkConnector.Core.Log.Implementation;
using Rego.LinkConnector.Core.Actions.DTO;
using System.Data;

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
        /// ILog implementation
        /// </summary>
        private ILog _log;

        /// <summary>
        /// Constant with actions query
        /// </summary>
        private const string QUERY_REGO_FLOW_ACTIONS = "rego_flow_actions";

        /// <summary>
        /// Constant with action parameters
        /// </summary>
        private const string QUERY_REGO_FLOW_PARAMETERS = "rego_flow_actions";

        /// <summary>
        /// Constructor
        /// </summary>
        public XOGAuthentication()
        {
            this._coreResourcesBLL = new CoreResourcesBLL();
            this._CAPPMResourcesBLL = new CAPPMResourcesBLL();
            this._log = new Log();
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
                    this._log.WriteLogError(this._coreResourcesBLL.GetResource("ERROR_NULL_AUTHORIZATION_HEADERS"));
                    return null;
                }

                string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(headers.Authorization.Parameter));

                string[] authenticationParams = decodedAuthenticationToken.Split('|');

                if (authenticationParams.Length != 2)
                {
                    this._log.WriteLogError(this._CAPPMResourcesBLL.GetResource("ERROR_URL_USER_SEPARATED_BY_PIPE"));
                    return null;
                }

                string url = authenticationParams[0],
                       usernameAndPassword = authenticationParams[1];

                int index = usernameAndPassword.IndexOf(':');

                string username = usernameAndPassword.Substring(0, index),
                       password = usernameAndPassword.Substring(username.Length + 1);

                if (!string.IsNullOrEmpty(url))
                {
                    this._log.WriteLogInfo(string.Format(this._coreResourcesBLL.GetResource("INFO_URL"),
                                                         url));
                }

                if (!string.IsNullOrEmpty(username))
                {
                    string encryptedUser;

                    if (username.Length <= 3)
                    {
                        encryptedUser = "***";
                    }
                    else
                    {
                        encryptedUser = username.Substring(0, 3);
                        encryptedUser += new string('*', username.Substring(3).Length);
                    }

                    this._log.WriteLogInfo(string.Format(this._coreResourcesBLL.GetResource("INFO_USER"),
                                                         encryptedUser));
                }

                if (string.IsNullOrEmpty(url) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password))
                {
                    this._log.WriteLogError(this._CAPPMResourcesBLL.GetResource("ERROR_URL_USER_SEPARATED_BY_PIPE"));
                    return null;
                }
                
                return new AuthenticationParametersDTO
                {
                    URL = url,
                    Username = username,
                    Password = password
                };
            }
            catch (Exception ex)
            {
                this._log.WriteLogError(string.Format(this._coreResourcesBLL.GetResource("ERROR_METHOD_EXCEPTION"),
                                                      "GetAuthenticationParametersFromRequestHeaders()",
                                                      ex.ToString()));
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
            bool unauthorized = false,
                 writeLog = true;

            try
            {
                if (authenticationParametersDTO == null)
                {
                    unauthorized = true;
                    writeLog = false;
                    throw new Exception(this._coreResourcesBLL.GetResource("ERROR_AUTHENTICATION_FAILED"));
                }

                if (!authenticationParametersDTO.URL.ToLower().Contains("/niku/xog"))
                {
                    unauthorized = true;
                    writeLog = false;
                    this._log.WriteLogError(string.Format(this._CAPPMResourcesBLL.GetResource("ERROR_INVALID_CAPPM_URL"),
                                                          authenticationParametersDTO.URL));
                    throw new Exception(string.Format(this._CAPPMResourcesBLL.GetResource("ERROR_INVALID_CAPPM_URL"),
                                                      ""));
                }

                this._xogClient = new XogClient(authenticationParametersDTO.URL);
                string sessionID = null;

                try
                {
                    sessionID = this._xogClient.GetSessionId(authenticationParametersDTO.Username,
                                                             authenticationParametersDTO.Password);
                }
                catch (XogClientLoginException)
                {
                    unauthorized = true;
                    throw;
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

                if (writeLog)
                {
                    this._log.WriteLogError(string.Format(this._coreResourcesBLL.GetResource("ERROR_METHOD_EXCEPTION"),
                                                          "EndPointAuthenticate()",
                                                          message));
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
            try
            {
                if (this._xogClient != null)
                {
                    this._xogClient.DisposeSession();
                }
            }
            catch (Exception ex)
            {
                this._log.WriteLogError(string.Format(this._coreResourcesBLL.GetResource("ERROR_METHOD_EXCEPTION"),
                                                      "EndPointLogOut()",
                                                      ex.ToString()));
            }
        }

        /// <summary>
        /// Gets the datasource available actions
        /// </summary>
        /// <returns></returns>
        public IList<ActionDTO> GetActiveActions()
        {
            try
            {
                IList<string> columns = new List<string>();
                columns.Add("action_id");
                columns.Add("action_name");

                IDictionary<string, string> orderBy = new Dictionary<string, string>();
                orderBy.Add("action_sequence", "asc");

                DataTable result = this._xogClient.GetQueryData(QUERY_REGO_FLOW_ACTIONS,
                                                                new Dictionary<string, string>(),
                                                                null,
                                                                orderBy,
                                                                null);

                if (result == null ||
                    result.Rows.Count == 0)
                {
                    return new List<ActionDTO>();
                }

                IList<ActionDTO> actionsDTO = new List<ActionDTO>();

                foreach (DataRow row in result.Rows)
                {
                    actionsDTO.Add(new ActionDTO
                    {
                        Id = row["action_id"].ToString(),
                        Name = row["action_name"].ToString()
                    });
                }

                return actionsDTO;
            }
            catch (Exception ex)
            {
                this._log.WriteLogError(string.Format(this._coreResourcesBLL.GetResource("ERROR_METHOD_EXCEPTION"),
                                                      "GetActiveActions()",
                                                      ex.ToString()));

                throw;
            }
        }
        #endregion
    }
}
