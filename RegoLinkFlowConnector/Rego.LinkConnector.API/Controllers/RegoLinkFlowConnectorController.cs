using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rego.LinkConnector.Core.Authentication.Contracts;
using Rego.LinkConnector.Core.Authentication.DTO;
using System;
using System.Collections.Generic;
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
        /// IAuth implementation
        /// </summary>
        IAuth _authentication;

        #region Public Methods
        /// <summary>
        /// Available actions HTTP request
        /// </summary>
        /// <returns>HTTP response with available actions</returns>
        public HttpResponseMessage GetActions()
        {
            try
            {
                HttpResponseMessage response = this.Authenticate();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response;
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
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.ToString()),
                    StatusCode = HttpStatusCode.InternalServerError
                };
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
        public HttpResponseMessage UpdateEndpoint()
        {
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
        #endregion

        #region Private Methods
        /// <summary>
        /// Authenticates the user in the destination host
        /// </summary>
        /// <returns>Authentication response</returns>
        private HttpResponseMessage Authenticate()
        {
            AuthenticationParametersDTO authenticationParametersDTO = this._authentication.GetAuthenticationParametersFromRequestHeaders(Request.Headers);

            return this._authentication.EndPointAuthenticate(authenticationParametersDTO);
        }

        /// <summary>
        /// Closes the session in the destination host
        /// </summary>
        private void Logout()
        {
            this._authentication.EndPointLogOut();
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
        #endregion
    }
}
