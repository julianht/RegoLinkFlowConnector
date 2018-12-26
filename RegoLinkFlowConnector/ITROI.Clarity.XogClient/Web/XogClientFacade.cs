using ITROI.Clarity.XogClient.DTO;
using System.Collections.Generic;
using System.Data;
using System;
using ITROI.Clarity.XogClient.Contracts;

namespace ITROI.Clarity.XogClient.Web
{
    /// <summary>
    /// Service for XogClientFacade
    /// </summary>
    public class XogClientFacade : IXogClient
    {
        /// <summary>
        /// XOG client
        /// </summary>
        private XogClient _xogClient;

        /// <summary>
        /// XOG client session Id
        /// </summary>
        private string _sessionId;

        /// <summary>
        /// XOG session ID
        /// </summary>
        public string SessionId { get { return _sessionId; } }

        /// <summary>
        /// Clarity URL
        /// </summary>
        string _url;

        /// <summary>
        /// Clarity User
        /// </summary>
        string _user;

        /// <summary>
        /// Clarity user password
        /// </summary>
        string _password;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">PPMVendor url</param>
        /// <param name="user">PPMVendor username</param>
        /// <param name="password">PPMVendor password</param>
        public XogClientFacade(string url, 
                               string user, 
                               string password)
        {
            _url = url;
            _user = user;
            _password = password;

            InternalInitialize();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">PPMVendor url</param>
        /// <param name="user">PPMVendor username</param>
        /// <param name="password">PPMVendor password</param>
        /// <param name="sessionIdCached">User session id</param>
        public XogClientFacade(string url, 
                               string user, 
                               string password, 
                               string sessionIdCached)
        {
            _url = url;
            _user = user;
            _password = password;
            _sessionId = sessionIdCached;

            InternalInitialize();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="dataSourceEndPointDTO">IDataSourceEndPointDTO implementation</param>
        /// <param name="user">PPMVendor username</param>
        /// <param name="password">PPMVendor password</param>
        /// <param name="sessionIdCached">User session id</param>
        public XogClientFacade(IDataSourceEndPointDTO dataSourceEndPointDTO,
                               string user,
                               string password, 
                               string sessionIdCached)
        {
            this._user = user;
            this._password = password;
            this._sessionId = sessionIdCached;
            this._xogClient = new XogClient(dataSourceEndPointDTO);
            this.SetSessionId();
        }

        /// <summary>
        /// Components initialization
        /// </summary>
        private void InternalInitialize()
        {
            InitializeXogClient();
        }

        /// <summary>
        /// XOG client variables initialization
        /// </summary>
        private void InitializeXogClient()
        {
            this._xogClient = new XogClient(_url);
            this.SetSessionId();
        }

        /// <summary>
        /// Saves SessionId values
        /// </summary>
        private void SetSessionId()
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                _sessionId = _xogClient.GetSessionId(_user, _password);
            }

            _xogClient.SetSessionId(_sessionId);         
        }

        #region IXogClient implementation
        /// <summary>
        /// Get clarity session Id
        /// </summary>
        /// <param name="user">Clarity username</param>
        /// <param name="password">Clarity password</param>
        /// <returns>Session Id for the clarity username</returns>
        public string GetSessionId(string user, 
                                   string password)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                return _xogClient.GetSessionId(user, password);
            }

            return _sessionId;
        }

        /// <summary>
        /// Assigns the sessionId to the XOG class instance
        /// </summary>
        /// <param name="sessionId">Session Id to assign</param>
        /// <returns>Assigned session Id</returns>
        public string SetSessionId(string sessionId)
        {
            _sessionId = sessionId;
            return _xogClient.SetSessionId(sessionId);
        }

        /// <summary>
        /// Get the list of queries defined in the PPMVendor
        /// </summary>
        /// <returns>List with tue PPMVendor queries names</returns>
        public IList<string> GetQueryList()
        {
            return _xogClient.GetQueryList();
        }

        /// <summary>
        /// Get columns information of a query
        /// </summary>
        /// <param name="queryName">Name of the query to extract the columns information</param>
        /// <returns>Datatable with the query columns information</returns>
        public DataTable GetQueryColumns(string queryName)
        {
            return _xogClient.GetQueryColumns(queryName);
        }

        /// <summary>
        /// Get a filtered query data
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with the columns filter</param>
        /// <param name="fields">Fields of the query to return</param>
        /// <returns>Datatable with the query results</returns>
        public DataTable GetQueryData(string queryName,
                                      IDictionary<string, string> filters,
                                      IEnumerable<string> fields)
        {
            return _xogClient.GetQueryData(queryName, filters, fields);
        }

        /// <summary>
        /// Get a filtered query data
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with the columns filter</param>
        /// <returns>Datatable with the query results</returns>
        public System.Data.DataTable GetQueryData(string queryName, 
                                                  IDictionary<string, string> filters)
        {
            return _xogClient.GetQueryData(queryName, filters);
        }

        /// <summary>
        /// Executes the XML and returns the XOG response as a dataset
        /// </summary>
        /// <param name="xml">XML to execute</param>
        /// <returns>Dataset with the executed XML response</returns>
        public DataSet GetDataSetResponseXog(string xml)
        {
            return _xogClient.GetDataSetResponseXog(xml);
        }

        /// <summary>
        /// Executes the XML and returns the XOG response as a string
        /// </summary>
        /// <param name="xml">XML to execute</param>
        /// <returns>String with the executed XML response</returns>
        public string GetStrResponseXog(string xml)
        {
            return _xogClient.GetStrResponseXog(xml);
        }

        /// <summary>
        /// Executes the write XML
        /// </summary>
        /// <param name="xml">XML to execute</param>
        public void WriteXmlXog(string xml)
        {
            _xogClient.WriteXmlXog(xml);
        }

        /// <summary>
        /// Executes the write XML
        /// </summary>
        /// <param name="xml">XML to execute</param>
        public void WriteXmlXogHTTP(string xml)
        {
            _xogClient.WriteXmlXogHTTP(xml);
        }

        /// <summary>
        /// Get XOG response
        /// </summary>
        public string GetResponse()
        {
            return _xogClient.GetResponse();
        }

        /// <summary>
        /// Get XOG request
        /// </summary>
        /// <returns>String with the XOG request</returns>
        public string GetXMLRequest()
        {
            return this._xogClient.GetXMLRequest();
        }

        /// <summary>
        ///  Executes the XOG request and returns the XOG response as a string
        /// </summary>
        /// <param name="getQueryDataDTO">DTO with the XOG request</param>
        /// <returns>String with the executed XML response</returns>
        public string GetQueryDataXML(GetQueryDataDTO getQueryDataDTO) {
            return _xogClient.GetQueryDataXML(getQueryDataDTO);
        }

        /// <summary>
        /// Get query data with sorting
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with query filters {field, value}</param>
        /// <param name="fields">List of the fields to return</param>
        /// <param name="orderBy">Dictionary with the sorting definition {field, sortType}</param>
        /// <param name="sliceDTO">SliceDTO paging object</param>
        /// <returns>Datatable with the query results</returns>
        public DataTable GetQueryData(string queryName,
                                      IDictionary<string, string> filters,
                                      IEnumerable<string> fields,
                                      IDictionary<string, string> orderBy,
                                      SliceDTO sliceDTO)
        {
            return _xogClient.GetQueryData(queryName, 
                                           filters,
                                           fields,
                                           orderBy,
                                           sliceDTO);
        }

        public IDataSourceTableDataDTO GetDataToDataSourceTableDataDTO(string queryName,
                                                  IDictionary<string, string> filters,
                                                  IEnumerable<string> fields,
                                                  IDictionary<string, string> orderBy,
                                                  SliceDTO sliceDTO)
        {
            return _xogClient.GetDataToDataSourceTableDataDTO(queryName, filters, fields, orderBy, sliceDTO);
        }

        /// <summary>
        /// Clears clarity sessionId
        /// </summary>
        public void RemoveSessionIdFromCache()
        {
            throw new NotImplementedException();
        }

        public void DisposeSession()
        {
            if (!string.IsNullOrEmpty(this._sessionId))
            {
                this._xogClient.DisposeSession();
            }
        }
        #endregion IXogClient
    }
}