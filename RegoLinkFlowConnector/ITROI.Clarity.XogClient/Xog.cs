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
using Microsoft.Web.Services3;
using System.Configuration;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// XOG parameters
    /// </summary>
    public class Xog : XogBase, IXog
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
        /// Clarity SOAP client, based on .NET web services enhacement.
        /// </summary>
        XOGSoapClient _xogSoapClient;

        /// <summary>
        /// Get/set a dataset with the XOG response
        /// </summary>
        public DataSet DataSetResponse { get; set; }

        XmlDocument _xogResponseDocument = new XmlDocument();

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">CA PPM Url</param>
        public Xog(string url)
            : base(url)
        {
            Uri clarityURI = new Uri(url);
            _xogSoapClient = new XOGSoapClient(clarityURI);
            _xogResponseDocument = new XmlDocument();
        }
        #endregion Constructor

        #region Public functions

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
                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                SoapEnvelope soapRequestEnvelope = new SoapEnvelope();
                soapRequestEnvelope.Envelope.SetAttribute("xmlns:xog", "http://www.niku.com/xog");
                soapRequestEnvelope.Body.InnerXml = string.Format(@"
                                             <Login xmlns:xog='http://www.niku.com/xog'>
                                                <Username>{0}</Username>
                                                <Password>{1}</Password>
                                             </Login>", user, WebUtility.HtmlEncode(password));

                this.SetWebProxy(soapRequestEnvelope);

                SoapEnvelope soapResponseEnvelope = _xogSoapClient.DoSoapRequest(soapRequestEnvelope);
                Response = soapResponseEnvelope.Envelope.OwnerDocument.InnerXml;

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(Response);

                AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                {
                    base.WriteLog("XOG : GetSessionID() StatusXog. FAILURE Response: " + Response);
                    throw new XogClientLoginException(Message);
                }

                return _sessionId;
            }
            catch (Exception ex)
            {
                base.WriteLogError("XOG : GetSessionID() Message= " + ex.Message + " -- StackTrace= " + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Build Xml complete and execute xog
        /// </summary>
        /// <param name="xml">Xml NikuDataBus</param>
        public void GetXmlXogResponse(string xml)
        {
            try
            {
                SendXmlRequest(xml);
                CheckResponseStatus();

                if (Status == StatusXog.FAILURE)
                {
                    base.WriteLog("XOG : GetXmlXogResponse() StatusXog. FAILURE XML= " + xml + " -- Response= " + Response);
                    throw new XogClientFailureException(Message);
                }
            }
            catch (XogClientLoginException xce)
            {
                base.WriteLogError("XOG : GetXmlXogResponse() XogClientLoginException : Message= " + xce.Message + " -- StackTrace= " + xce.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                base.WriteLogError("XOG : GetXmlXogResponse() Exception : Message= " + ex.Message + " -- StackTrace= " + ex.StackTrace);
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
                SendXmlRequest(xml);
                AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                {
                    base.WriteLog("XOG : ExecXmlXog() StatusXog. FAILURE XML= " + xml + " -- Response= " + Response);
                    throw new XogClientFailureException(Message);
                }
            }
            catch (XogClientLoginException xce)
            {
                Response = xce.Message;
                base.WriteLogError("XOG : ExecXmlXog() XogClientLoginException: Message= " + xce.Message + " -- StackTrace= " + xce.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                Response = ex.Message;
                base.WriteLogError("XOG : ExecXmlXog() Exception: Message= " + ex.Message + " -- StackTrace= " + ex.StackTrace + " -- XML= " +xml );
                throw;
            }
        }

        /// <summary>
        /// Build Xml complete and execute xog
        /// </summary>
        /// <param name="xml">Xml NikuDataBus</param>
        public void ExecXmlXogHTTP(string xml)
        {
            try
            {
                string soapMessage =
                        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xog=\"http://www.niku.com/xog\">" +
                        "	<soapenv:Header>" +
                        "			<xog:Auth>" +
                        "			<xog:SessionID>" + SessionId + "</xog:SessionID>" +
                        "			</xog:Auth>" +
                        "	</soapenv:Header>" +
                        "	<soapenv:Body>" +
                                xml +
                        "	</soapenv:Body>" +
                        "</soapenv:Envelope>";

                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                byte[] requestBuffer = Encoding.ASCII.GetBytes(soapMessage);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Method = "POST";
                webRequest.ContentType = "text/xml";
                webRequest.ContentLength = requestBuffer.Length;
                webRequest.Timeout = Timeout.Infinite;
                webRequest.ReadWriteTimeout = Timeout.Infinite;

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

                this.AnalizeResponse();

                if (Status == StatusXog.FAILURE)
                {
                    base.WriteLog("XOG : ExecXmlXogHTTP() StatusXog. FAILURE XML= " + xml + " -- Response= " + Response);
                    throw new XogClientFailureException(Message);
                }
            }
            catch (XogClientLoginException xce)
            {
                base.WriteLogError("XOG : ExecXmlXogHTTP() XogClientLoginException: Message= " + xce.Message + " -- StackTrace= " + xce.StackTrace + " -- XML= " + xml);
                throw;
            }
            catch (Exception ex)
            {
                base.WriteLogError("XOG : ExecXmlXogHTTP() Exception : Message= " + ex.Message + " -- StackTrace= " + ex.StackTrace + " -- XML= " + xml);
                throw;
            }
        }

        /// <summary>
        /// Logout of the current SessionID
        /// </summary>
        public void DisposeSession()
        {
            try
            {
                string soapMessage =
                        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xog=\"http://www.niku.com/xog\">" +
                        "	<soapenv:Header>" +
                        "			<xog:Auth>" +
                        "			<xog:SessionID>" + this._sessionId + "</xog:SessionID>" +
                        "			</xog:Auth>" +
                        "	</soapenv:Header>" +
                        "	<soapenv:Body>" +
                        "   <xog:Logout/>" + 
                        "	</soapenv:Body>" +
                        "</soapenv:Envelope>";

                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                byte[] requestBuffer = Encoding.ASCII.GetBytes(soapMessage);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Method = "POST";
                webRequest.ContentType = "text/xml";
                webRequest.ContentLength = requestBuffer.Length;
                webRequest.Timeout = Timeout.Infinite;
                webRequest.ReadWriteTimeout = Timeout.Infinite;

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

                this.AnalizeResponse();

                if (this.Status == StatusXog.FAILURE)
                {
                    base.WriteLog("DisposeSession(): " + Response);
                    throw new XogClientFailureException(Message);
                }
            }
            catch (Exception ex)
            {
                base.WriteLogError("DisposeSession(): " + ex.ToString());
                throw;
            }
        }
        #endregion Public functions

        #region Private functions

            /// <summary>
            /// Executes the xml xog request
            /// </summary>
            /// <param name="xml">Xml NikuDataBus</param>
            private void SendXmlRequest(string xml)
        {
            ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            SoapEnvelope soapRequestEnvelope = new SoapEnvelope();
            soapRequestEnvelope.Envelope.SetAttribute("xmlns:xog", "http://www.niku.com/xog");

            soapRequestEnvelope.CreateHeader();
            soapRequestEnvelope.Header.InnerXml = string.Format(@"
        	                                        <xog:Auth>
			                                            <xog:SessionID>{0}</xog:SessionID>
		                                            </xog:Auth>
                                                  ", this._sessionId);

            soapRequestEnvelope.Body.InnerXml = xml;

            this.SetWebProxy(soapRequestEnvelope);
            SoapEnvelope soapResponseEnvelope = _xogSoapClient.DoSoapRequest(soapRequestEnvelope);
            Response = soapResponseEnvelope.Envelope.OwnerDocument.InnerXml;
        }

        /// <summary>
        /// Analize response and set Status, Message
        /// </summary>
        private void AnalizeResponse()
        {
            CheckResponseStatus();

            BuildDataSetXml();
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

        /// <summary>
        /// Sets a WebProxy object to the SoapEnveloe object
        /// </summary>
        /// <param name="soapEnvelope">SoapEnvelope object</param>
        private void SetWebProxy(SoapEnvelope soapEnvelope)
        {
            WebProxy proxy = base.GetWebProxy();

            if (proxy == null)
            {
                return;
            }

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            soapEnvelope.Context.WebRequest = webRequest;
            soapEnvelope.Context.WebRequest.Proxy = proxy;
        }
        #endregion Private functions
    }
}