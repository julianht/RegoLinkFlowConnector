using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rego.LinkConnector.Core.Authentication.DTO;
using Rego.LinkConnector.Core.Implementation;
using Rego.LinkConnector.Core.Log.Contracts;
using Rego.LinkConnector.Core.Log.Implementation;
using Rego.LinkConnector.Core.Resources.Contracts;
using Rego.LinkConnector.Core.Resources.Implementation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Rego.LinkConnector.API.Controllers
{
    /// <summary>
    /// Controller for link connector actions
    /// </summary>
    public class RegoLinkFlowConnectorController : ApiController
    {
        /// <summary>
        /// IDataSourceConnector implementation
        /// </summary>
        IDataSourceConnector _dataSourceConnector;

        /// <summary>
        /// Ilog implementation
        /// </summary>
        ILog _log = new Log();

        /// <summary>
        /// ICoreResourcesBLL implementation
        /// </summary>
        ICoreResourcesBLL _coreResourcesBLL = new CoreResourcesBLL();

        #region Public Methods
        /// <summary>
        /// Available actions HTTP request
        /// </summary>
        /// <returns>HTTP response with available actions</returns>
        public HttpResponseMessage GetActions()
        {
            try
            {
                this._log.WriteLogInfo(this._coreResourcesBLL.GetResource("INFO_GET_ACTIONS_HTTP_REQUEST"));

                this.InitializeProperties();

                HttpResponseMessage response = this.Authenticate();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return this.FormatErrorHttpResponseMessage(response);
                }

                JArray objects = this.GetAvailableObjectsNames();

                this.Logout();

                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(objects),
                                                Encoding.UTF8,
                                                "application/json")
                };
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message),
                    StatusCode = HttpStatusCode.InternalServerError
                };

                return this.FormatErrorHttpResponseMessage(response);
            }
        }

        /// <summary>
        /// Action parameters definition HTTP request
        /// </summary>
        /// <returns>HTTP response with action parameters definition</returns>
        public HttpResponseMessage GetParameters(string id)
        {
            try
            {
                this._log.WriteLogInfo(string.Format(this._coreResourcesBLL.GetResource("INFO_GET_PARAMETERS_HTTP_REQUEST"), 
                                                     id));

                this.InitializeProperties();

                HttpResponseMessage response = this.Authenticate();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response;
                }

                JObject type = new JObject();
                type.Add("type", "string");
                JObject prop = new JObject();
                prop.Add("ProjectCode", type);
                prop.Add("ProjectName", type);

                JObject item = new JObject();
                item.Add("type", "object");
                item.Add("properties", prop);

                JObject jsonObject = new JObject();
                jsonObject.Add("type", "array");
                jsonObject.Add("items", item);

                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(jsonObject),
                                                Encoding.UTF8,
                                                "application/json")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.ToString()),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Update/create object instance
        /// </summary>
        /// <returns>HTTP response with update/create result</returns>
        [HttpPost]
        public HttpResponseMessage ExecuteAction()
        {
            try
            {
                this._log.WriteLogInfo(this._coreResourcesBLL.GetResource("INFO_EXECUTE_ACTION_HTTP_REQUEST"));

                this.InitializeProperties();

                HttpResponseMessage response = this.Authenticate();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response;
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent("OK")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.ToString()),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Authenticates the user in the destination host
        /// </summary>
        /// <returns>Authentication response</returns>
        private HttpResponseMessage Authenticate()
        {
            this._log.WriteLogInfo(this._coreResourcesBLL.GetResource("INFO_ENDPOINT_AUTHENTICATE_EXECUTION"));

            AuthenticationParametersDTO authenticationParametersDTO = this._dataSourceConnector.GetAuthenticationParametersFromRequestHeaders(Request.Headers);

            return this._dataSourceConnector.EndPointAuthenticate(authenticationParametersDTO);
        }

        /// <summary>
        /// Closes the session in the destination host
        /// </summary>
        private void Logout()
        {
            this._log.WriteLogInfo(this._coreResourcesBLL.GetResource("INFO_ENDPOINT_LOGOUT_EXECUTION"));

            this._dataSourceConnector.EndPointLogOut();
        }

        /// <summary>
        /// Gets the available objects names
        /// </summary>
        /// <returns>List with available objects names</returns>
        private JArray GetAvailableObjectsNames()
        {
            JArray json = new JArray();
            JObject jsonObject = new JObject();
            jsonObject.Add("id", "1");
            jsonObject.Add("name", "Projects");
            json.Add(jsonObject);

            jsonObject = new JObject();
            jsonObject.Add("id", "2");
            jsonObject.Add("name", "Ideas");
            json.Add(jsonObject);

            jsonObject = new JObject();
            jsonObject.Add("id", "3");
            jsonObject.Add("name", "Resources");
            json.Add(jsonObject);

            return json;
        }

        /// <summary>
        /// Formats the http error message with json structure
        /// </summary>
        /// <param name="response">Http response to format</param>
        /// <returns>Http response with json format</returns>
        private HttpResponseMessage FormatErrorHttpResponseMessage(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }

            JObject error = new JObject();
            error.Add("code", (int)response.StatusCode);
            error.Add("message", response.StatusCode + ". " + response.Content.ReadAsStringAsync().Result);

            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(error),
                                            Encoding.UTF8,
                                            "application/json"),
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Initialize class properties (Must be called from every web method)
        /// </summary>
        private void InitializeProperties()
        {
            try
            {
                this._dataSourceConnector = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["LinkConnector:IDataSourceConnectorImplementation"], true)) as IDataSourceConnector;
            }
            catch (Exception ex)
            {
                this._log.WriteLogError(string.Format(this._coreResourcesBLL.GetResource("ERROR_LOADING_DATASOURCE_CONNECTOR_ASSEMBLY"),
                                                      ex.ToString()));
                throw ex;
            }
        }
        #endregion
    }
}
