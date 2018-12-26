using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using ITROI.Clarity.XogClient.Exceptions;
using System.Threading;
using System.Configuration;
using ITROI.Clarity.XogClient.Contracts;
using Newtonsoft.Json.Linq;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// XOG parameters
    /// </summary>
    public class XogHttpWebRequest : XogBase, IXog
    {
        /// <summary>
        /// Session id
        /// </summary>
        private string _sessionId;

        /// <summary>
        /// Get/set XOG status
        /// </summary>
        public StatusXog Status { get; set; }

        /// <summary>
        /// Get/set user session id
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Get/set XOG response
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Get/set XOG response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get/set a dataset with the XOG response
        /// </summary>
        public DataSet DataSetResponse { get; set; }

        XmlDocument _xogResponseDocument = new XmlDocument();

        /// <summary>
        /// IDataSourceEndPointDTO implementation with endpoint definition
        /// </summary>
        private IDataSourceEndPointDTO _dataSourceEndPointDTO;

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">CA PPM Url</param>
        public XogHttpWebRequest(string url)
            : base(url)
        {
            _xogResponseDocument = new XmlDocument();
            this._dataSourceEndPointDTO = null;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">CA PPM Url</param>
        public XogHttpWebRequest(IDataSourceEndPointDTO dataSourceEndPointDTO)
            : base(dataSourceEndPointDTO.EndPointURL.TrimEnd('/') + "/api/xog")
        {
            _xogResponseDocument = new XmlDocument();
            this._dataSourceEndPointDTO = dataSourceEndPointDTO;
        }
        #endregion Constructor

        #region Public methods

        public XmlDocument XogResponseDocument
        {
            get
            {
                return _xogResponseDocument;
            }
        }

        /// <summary>
        /// Get clarity session Id
        /// </summary>
        /// <param name="user">Clarity username</param>
        /// <param name="password">Clarity password</param>
        /// <returns>Session Id for the clarity username</returns>
        public string GetSessionID(string user, 
                                   string password)
        {
            try
            {
                string soapMessage =
                    "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xog=\"http://niku.com/xog.xsd\">" +
                        "<soapenv:Header/>" +
                        "<soapenv:Body>" +
                            "<xog:Login>" +
                                 "<xog:Username>" + user + "</xog:Username><xog:Password>" + password + "</xog:Password>" +
                            "</xog:Login>" +
                        "</soapenv:Body>" +
                    "</soapenv:Envelope>";

                ExecXog(soapMessage);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(Response);

                AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                    throw new XogClientLoginException(Message);

                return _sessionId;
            }
            catch (Exception ex)
            {
                base.WriteLogError("Exception: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Build Xml complete and execute xog
        /// </summary>
        /// <param name="xml">Xml NikuDataBus</param>
        public void ExecXmlXog(string xml)
        {
            try
            {
                xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xog=\"http://www.niku.com/xog\">                                                              " +
                      "	<soapenv:Header>                                                                                                                                                               " +
                      "			<xog:Auth>                                                                                                                                                             " +
                      "			<xog:SessionID>" + SessionId + "</xog:SessionID>                                                                                                                            " +
                      "			</xog:Auth>                                                                                                                                                            " +
                      "	</soapenv:Header>					                                                                                                                                       " +
                      "	<soapenv:Body>                                                                                                                                                             " +
                                xml +
                      "	</soapenv:Body>                                                                                                                                                                " +
                      "</soapenv:Envelope>";
                
                ExecXog(xml);
                AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                    throw new XogClientFailureException(Message);
            }
            catch (XogClientLoginException)
            {
                throw;
            }
        }

        /// <summary>
        /// Build Xml complete and execute xog
        /// </summary>
        /// <param name="xml">Xml NikuDataBus</param>
        public void ExecXmlXogHTTP(string xml)
        {
            ExecXmlXog(xml);
        }

        /// <summary>
        /// Build Xml complete and execute xog
        /// </summary>
        /// <param name="xml">Xml NikuDataBus</param>
        public void GetXmlXogResponse(string xml)
        {
            try
            {
                ExecXmlXog(xml);
                CheckResponseStatus();

                if (Status == StatusXog.FAILURE)
                {
                    throw new XogClientFailureException(Message);
                }
            }
            catch (XogClientLoginException)
            {
                throw;
            }
        }

        /// <summary>
        /// Closes open session
        /// </summary>
        public void DisposeSession()
        {
            try
            {
                string xog =
                        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xog=\"http://www.niku.com/xog\">" +
                        "	<soapenv:Header>" +
                        "			<xog:Auth>" +
                        "			<xog:SessionID>" + this.SessionId + "</xog:SessionID>" +
                        "			</xog:Auth>" +
                        "	</soapenv:Header>" +
                        "	<soapenv:Body>" +
                        "   <xog:Logout/>" +
                        "	</soapenv:Body>" +
                        "</soapenv:Envelope>";

                ExecXog(xog);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(Response);

                AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                {
                    throw new XogClientLoginException(Message);
                }
            }
            catch (Exception ex)
            {
                base.WriteLogError("Exception: " + ex.Message);
                throw;
            }
        }
        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Execute xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        private void ExecXog(string xml)
        {
            ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.Method = "POST";

            byte[] requestBuffer;

            if (this._dataSourceEndPointDTO == null ||
                !this._dataSourceEndPointDTO.IsDefined)
            {
                webRequest.ContentType = "text/xml";
                requestBuffer = Encoding.ASCII.GetBytes(xml);
            }
            else
            {
                webRequest.ContentType = "application/json";
                JObject json = new JObject();
                json["XOGXMLRequest"] = xml;
                json["APIKey"] = this._dataSourceEndPointDTO.APIKey;
                requestBuffer = Encoding.ASCII.GetBytes(json.ToString());
            }

            webRequest.ContentLength = requestBuffer.Length;
            webRequest.ReadWriteTimeout = Timeout.Infinite;

            string ecBossXOGTimeout = ConfigurationManager.AppSettings["ecBossXOGTimeout"];

            if (string.IsNullOrEmpty(ecBossXOGTimeout))
            {
                //1 hour
                webRequest.Timeout = 3600000;
            }
            else
            {
                webRequest.Timeout = Convert.ToInt32(ecBossXOGTimeout);
            }

            //webRequest.KeepAlive = false;

            base.WriteLog("Starts XogHttpWebRequest");

            webRequest.Proxy = base.GetWebProxy();

            Stream postStream = webRequest.GetRequestStream();
            postStream.Write(requestBuffer, 0, requestBuffer.Length);
            postStream.Close();
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            Response = result;
            base.WriteLog("Request Response: " + Response);
        }

        private void CheckResponseStatus()
        {
            Status = StatusXog.SUCCESS;

            //XML structure rebuild due to parser exception
            Response = Response.Replace("xbl:startProcess", "startProcess");

            _xogResponseDocument.LoadXml(Response);

            XmlNodeList nodeList = _xogResponseDocument.GetElementsByTagName("Status");

            if (nodeList.Count != 0)
            {
                Status = (StatusXog)Enum.Parse(typeof(StatusXog), nodeList.Item(0).Attributes["state"].InnerText);
            }

            nodeList = _xogResponseDocument.GetElementsByTagName("ErrorInformation");
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode node in nodeList)
            {
                if (node["Severity"] != null)
                    sb.Append("Severity: " + node["Severity"].InnerText + "\n");
                if (node["Description"] != null)
                    sb.Append("Description: " + node["Description"].InnerText + "\n");
                if (node["Exception"] != null)
                    sb.Append("Exception: " + node["Exception"].InnerText + "\n");
                sb.Append("\n");
                sb.Append("\n");
            }

            Message = sb.ToString();
            XmlNodeList nodeSession = _xogResponseDocument.GetElementsByTagName("SessionID");
            if (nodeSession.Count != 0)
            {
                _sessionId = nodeSession.Item(0).InnerText;
            }
        }

        /// <summary>
        /// Execute xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        private void ExecXogOri(string xml)
        {
            byte[] requestBuffer = Encoding.ASCII.GetBytes(xml);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";
            webRequest.ContentLength = requestBuffer.Length;

            Stream postStream = webRequest.GetRequestStream();
            postStream.Write(requestBuffer, 0, requestBuffer.Length);
            postStream.Close();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            Stream responseStream = webResponse.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            Response = responseReader.ReadToEnd();
            responseReader.Close();
        }

        /// <summary>
        /// Analize response and set Status, Message
        /// </summary>
        private void AnalizeResponse()
        {
            CheckResponseStatus();
            BuildDataSetXml();
        }

        /// <summary>
        /// Convert xml to DataSet
        /// </summary>
        private void BuildDataSetXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Response);

            XmlReader xmlReader = new XmlNodeReader(doc);
            DataSetResponse = new DataSet();
            DataSetResponse.ReadXml(xmlReader);
        }

        #endregion Private functions
    }
}