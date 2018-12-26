using ITROI.Clarity.XogClient.Contracts;
using ITROI.Clarity.XogClient.DTO;
using ITROI.Clarity.XogClient.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// XOG client services
    /// </summary>
    public class XogClient : IXogClient
    {
        #region attributes
        /// <summary>
        /// Url
        /// </summary>
        string _url;

        /// <summary>
        /// XOG request object
        /// </summary>
        IXog _xog;

        /// <summary>
        /// XML XOG request
        /// </summary>
        private string _xmlRequest;
        #endregion attributes

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="url">Url XOG</param>
        public XogClient(string url)
        {
            _url = url;
            _xog = XogFactory.GetXogImplementation(url);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="dataSourceEndPointDTO">IDataSourceEndPointDTO implementation</param>
        public XogClient(IDataSourceEndPointDTO dataSourceEndPointDTO)
        {
            this._url = dataSourceEndPointDTO.EndPointURL.TrimEnd('/') + "/api/xog";
            this._xog = XogFactory.GetXogImplementation(dataSourceEndPointDTO);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Get clarity session Id
        /// </summary>
        /// <param name="user">Clarity username</param>
        /// <param name="password">Clarity password</param>
        /// <returns>Session Id for the clarity username</returns>
        public string GetSessionId(string user, string password)
        {
            return _xog.GetSessionID(user, password);
        }

        /// <summary>
        /// Assigns the sessionId to the XOG class instance
        /// </summary>
        /// <param name="sessionId">Session Id to assign</param>
        /// <returns>Assigned session Id</returns>
        public string SetSessionId(string sessionId)
        {
            return _xog.SessionId = sessionId;
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Remove substring "/xog"
        /// </summary>
        /// <param name="servername">Server Name</param>
        /// <returns>Server Name without "/xog"</returns>
        private String ParseServer(string servername)
        {
            servername = servername.Substring(0, servername.IndexOf("/xog"));

            return servername;
        }

        /// <summary>
        /// Get data query
        /// </summary>
        /// <param name="queryName"></param>
        /// <param name="filters">Filters {field, value}</param>
        /// <param name="fields">Fields DataTable</param>
        /// <param name="orderBy">Sorting fields { field, sortType }</param>
        /// <param name="sliceDTO">SliceDTO paging object</param>
        /// <returns>DataTable {param name="fields"}</returns>
        private DataTable GetQueryDataWithSorting(string queryName,
                                                  IDictionary<string, string> filters,
                                                  IEnumerable<string> fields,
                                                  IDictionary<string, string> orderBy,
                                                  SliceDTO sliceDTO)
        {
            try
            {
                StringBuilder sbFilters = new StringBuilder(),
                              sbSorting = new StringBuilder(),
                              sbSlice = new StringBuilder();

                if(filters != null)
                {
                    foreach (KeyValuePair<string, string> filter in filters)
                    {
                        sbFilters.Append("<" + filter.Key + ">" + this.EncodeQuerySpecialCharacters(filter.Key,
                                                                                                    filter.Value) + "</" + filter.Key + ">");
                    }
                }

                if (orderBy != null &&
                    orderBy.Any())
                {
                    sbSorting.Append("<Sort>");

                    foreach (KeyValuePair<string, string> sort in orderBy)
                    {
                        sbSorting.Append("<Column><Name>" + sort.Key + "</Name><Direction>" + sort.Value + "</Direction></Column>");
                    }

                    sbSorting.Append("</Sort>");
                }

                if (sliceDTO != null)
                {
                    sbSlice.Append("<Slice>");
                    sbSlice.Append("<Number>" + sliceDTO.Number + "</Number>");
                    sbSlice.Append("<Size>" + sliceDTO.Size + "</Size>");
                    sbSlice.Append("</Slice>");
                }

                this._xmlRequest =
                    "<Query xmlns=\"http://www.niku.com/xog/Query\">" +
                    "   <Code>" + queryName + "</Code>" +
                    "       <Filter>  " +
                                sbFilters.ToString() +
                    "       </Filter> " +
                            sbSorting.ToString() +
                            sbSlice.ToString() +
                    "</Query>";

                _xog.ExecXmlXog(this._xmlRequest);

                DataTable dt = _xog.DataSetResponse.Tables.Contains("Record") ? _xog.DataSetResponse.Tables["Record"] : new DataTable();

                //Property added to the datatable with the real query records lentgh
                dt.ExtendedProperties.Add("QueryRecordsLength", this.GetXOGResponseRecordsLength());

                if (fields == null)
                {
                    return dt;
                }

                DataTable dtResult = dt.Copy();
                foreach (DataColumn column in dt.Columns)
                {
                    if (fields.Contains(column.ColumnName))
                    {
                        continue;
                    }

                    dtResult.Columns.Remove(column.ColumnName);
                }

                return dtResult;
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Evaluates de XOG response string and extracts the total attribute
        /// </summary>
        /// <returns>Number of records in the XML response</returns>
        private int GetXOGResponseRecordsLength()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_xog.Response.ToLower());
            XmlNodeList nodeList = doc.GetElementsByTagName("slice");

            foreach (XmlNode node in nodeList)
            {
                if (node["total"] != null)
                {
                    return Convert.ToInt32(node["total"].InnerText);
                }
            }

            return 0;
        }

        /// <summary>
        /// Encodes query special characters
        /// </summary>
        /// <param name="queryColumn">Name of the column associated to the value</param>
        /// <param name="value">Value to make the replacement</param>
        /// <returns>Encoded column value</returns>
        private string EncodeQuerySpecialCharacters(string queryColumn,
                                                    string value)
        {
            try
            {
                value = value.Replace("&", "&amp;")
                             .Replace("<", "&lt;");

                return queryColumn.Length < 3 ||
                       queryColumn.Substring(queryColumn.Length - 3, 3) != "_in" ? value : value.Replace("'", "''");
            }
            catch
            {
                return value;
            }
        }
        #endregion Private Methods

        #region IXogClient functions
        /// <summary>
        /// Get Clarity Queries
        /// </summary>
        /// <returns>IEnumerable of queries name</returns>
        public IList<string> GetQueryList()
        {
            //we read the output from the wsdl queries list
            //we get the clarity URL
            string queryService = _url;
            string result = "";

            using (var client = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;

                //remove the last /xog string, to get http://<servername>/niku
                queryService = ParseServer(queryService);

                //we load the html ouput from http://<servername>/niku/wsdl/Query?tenantId=clarity
                result = client.DownloadString(queryService + "/wsdl/Query?tenantId=clarity");
            }

            IList<string> iListQueries = new List<string>();
            string iquery;
            string find_string = "'>";

            //we find all the queries ids and put them in a sortedlist (so we don't need to sort that information later)
            while (result.Contains(find_string))
            {
                iquery = result.Substring(result.IndexOf(find_string) + find_string.Length, result.IndexOf("</a>") - result.IndexOf(find_string) - find_string.Length);

                if (!iquery.Equals("(save as...)"))
                    iListQueries.Add(iquery);
                result = result.Substring(result.IndexOf("</a>")+4);
            }

            return iListQueries;
        }

        /// <summary>
        /// Get query columns
        /// </summary>
        /// <param name="queryName">Query name</param>
        /// <returns>DataTable {id,query_code,filter_association,filter_association_type,is_filterable,is_required,filter_type,lookup_type,data_type}</returns>
        public DataTable GetQueryColumns(string queryName)
        {
            try
            {
                Dictionary<string, string> filters = new Dictionary<string, string>();

                filters.Add("query_code", queryName);

                DataTable table = GetQueryData(Constants.QUERY_COLUMNS, filters);
                this.SetColumnsType(queryName, table);
                return table;
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Clarity Queries
        /// </summary>
        /// <returns>IEnumerable of queries name</returns>
        private void SetColumnsType(string queryName, DataTable table)
        {
            table.Columns.Add("data_type", typeof(String));

            string queryService = _url;
            string result = string.Empty;

            using (var client = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.ValidateServerCertificate;
                //remove the last /xog string, to get http://<servername>/niku
                queryService = ParseServer(queryService);
                //we load the html ouput from http://<servername>/niku/wsdl/Query?tenantId=clarity
                result = client.DownloadString(queryService + "/wsdl/Query/" + queryName + "?tenantId=clarity");
            }

            var xs = System.Xml.Linq.XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            var doc = System.Xml.Linq.XDocument.Parse(result);
            string elementName;
            DataRow[] foundRows;

            System.Xml.Linq.XElement elementTypes = null;

            foreach(var element in doc.Descendants(xs + "complexType"))
            {
                if (element.Attribute("name") == null)
                    continue;

                if (element.Attribute("name").Value.Equals(queryName + "Record"))
                {
                    elementTypes = element;
                    break;
                }
            }

            if (elementTypes == null)
                return;

            foreach (var elementType in elementTypes.Descendants(xs + "element"))
            {
                if (elementType.Attribute("name") == null)
                    continue;

                if (elementType.Attribute("type") == null)
                    continue;

                elementName = elementType.Attribute("name").Value;

                foundRows = table.Select("filter_association = '" + elementName + "'");

                if (foundRows.Length == 1)
                    foundRows[0]["data_type"] = elementType.Attribute("type").Value.Replace("xsd:", "");
            }

        }

        /// <summary>
        /// Get data query
        /// </summary>
        /// <param name="filters">Filters {field, value}</param>
        /// <param name="fields">Fields DataTable</param>
        /// <returns>DataTable {param name="fields"}</returns>
        public DataTable GetQueryData(string queryName, 
                                      IDictionary<string, string> filters, 
                                      IEnumerable<string> fields)
        {
            return this.GetQueryDataWithSorting(queryName,
                                                filters,
                                                fields,
                                                null,
                                                null);
        }

        /// <summary>
        /// Get query data as XML
        /// </summary>
        /// <param name="GetQueryDataDTO">DTO with query parameters</param>
        /// <returns>String with XOG response</returns>
        public string GetQueryDataXML(GetQueryDataDTO getQueryDataDTO)
        {
            try
            {
                StringBuilder sbFilters = new StringBuilder();
                if(getQueryDataDTO.DictionaryFilters != null)
                {
                    foreach (KeyValuePair<string, string> filter in getQueryDataDTO.DictionaryFilters)
                    {
                        sbFilters.Append("<" + filter.Key + ">" + this.EncodeQuerySpecialCharacters(filter.Key,
                                                                                                    filter.Value) + "</" + filter.Key + ">");
                    }
                }

                string slice = string.Empty;
                if (getQueryDataDTO.SliceDTO != null)
                {
                    slice = "<Slice>" +
                            "    <Number>" + getQueryDataDTO.SliceDTO.Number + "</Number>" +
                            "    <Size>" + getQueryDataDTO.SliceDTO.Size + "</Size>" + 
                            "</Slice>";
                }

                string sorting = string.Empty;
                if (getQueryDataDTO.SortDTO != null &&
                    getQueryDataDTO.SortDTO.Any())
                {
                    sorting = "<Sort>";

                    foreach (SortDTO sortDTO in getQueryDataDTO.SortDTO)
                    {
                        sorting += "<Column>" +
                                    "<Name>" + sortDTO.Name + "</Name>" +
                                    "<Direction>" + sortDTO.Direction + "</Direction>" +
                                   "</Column>";
                    }

                    sorting += "</Sort>";
                }

                this._xmlRequest =
                    "<Query xmlns=\"http://www.niku.com/xog/Query\">" +
                    "   <Code>" + getQueryDataDTO.QueryName + "</Code>" +
                    "       <Filter>  " +
                                sbFilters.ToString() +
                    "       </Filter> " +
                    slice +
                    sorting +                    
                    "</Query>";

                _xog.ExecXmlXog(this._xmlRequest);

                return _xog.Response;
            }
            catch (XogClientException)
            {
                throw;
            } 
        }

        /// <summary>
        /// Get data query
        /// </summary>
        /// <param name="filters">Filters {field, value}</param>
        /// <param name="fields">Fields DataTable</param>
        /// <returns>DataTable {all fields query}</returns>
        public DataTable GetQueryData(string queryName, 
                                      IDictionary<string, string> filters)
        {
            try
            {
                return this.GetQueryData(queryName, filters, null);
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Exec xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        /// <returns>DataSet result xml</returns>
        public DataSet GetDataSetResponseXog(string xml)
        {
            try
            {
                _xog.ExecXmlXog(xml);
                return _xog.DataSetResponse;
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Exec xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        /// <returns>String result xml</returns>
        public string GetStrResponseXog(string xml)
        {
            try
            {
                _xog.ExecXmlXog(xml);
                return _xog.Response;
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Write xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        public void WriteXmlXog(string xml)
        {
            try
            {
                _xog.ExecXmlXog(xml);
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Write xml XOG
        /// </summary>
        /// <param name="xml">Xml XOG</param>
        public void WriteXmlXogHTTP(string xml)
        {
            try
            {
                _xog.ExecXmlXogHTTP(xml);
            }
            catch (XogClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Get xog response
        /// </summary>
        public string GetResponse()
        {
            return _xog.Response;
        }

        /// <summary>
        /// Get XOG request
        /// </summary>
        /// <returns>String with the XOG request</returns>
        public string GetXMLRequest()
        {
            return this._xmlRequest;
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
            return this.GetQueryDataWithSorting(queryName,
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

            IDataSourceTableDataDTO dataSourceTableDataBaseDTO = new DataSourceTableDataBaseDTO();

            try
            {                                
                StringBuilder sbFilters = new StringBuilder(),
                              sbSorting = new StringBuilder(),
                              sbSlice = new StringBuilder();

                if(filters != null)
                {
                    foreach (KeyValuePair<string, string> filter in filters)
                    {
                        sbFilters.Append("<" + filter.Key + ">" + filter.Value + "</" + filter.Key + ">");
                    }
                }

                if (orderBy != null &&
                    orderBy.Any())
                {
                    sbSorting.Append("<Sort>");

                    foreach (KeyValuePair<string, string> sort in orderBy)
                    {
                        sbSorting.Append("<Column><Name>" + sort.Key + "</Name><Direction>" + sort.Value + "</Direction></Column>");
                    }

                    sbSorting.Append("</Sort>");
                }

                if (sliceDTO != null)
                {
                    sbSlice.Append("<Slice>");
                    sbSlice.Append("<Number>" + sliceDTO.Number + "</Number>");
                    sbSlice.Append("<Size>" + sliceDTO.Size + "</Size>");
                    sbSlice.Append("</Slice>");
                }

                string xml =
                    "<Query xmlns=\"http://www.niku.com/xog/Query\">" +
                    "   <Code>" + queryName + "</Code>" +
                    "       <Filter>  " +
                                sbFilters.ToString() +
                    "       </Filter> " +
                            sbSorting.ToString() +
                            sbSlice.ToString() +
                    "</Query>";

                _xog.GetXmlXogResponse(xml);

                XmlNodeList recordsElementsList = _xog.XogResponseDocument.DocumentElement.GetElementsByTagName("Record");

                if(recordsElementsList.Count == 0)
                {
                    return dataSourceTableDataBaseDTO;
                }

                XmlNode currentRecordNode = recordsElementsList.Item(0);

                int nodeCounter = 0;
                int columsToIgnoreCounter = 0;
                int[] columnsToIgnoreList = new int[currentRecordNode.ChildNodes.Count];                

                foreach (XmlNode columnNode in currentRecordNode)
                {
                    if (!fields.Contains(columnNode.Name))
                    {
                        columnsToIgnoreList[columsToIgnoreCounter] = nodeCounter;
                        columsToIgnoreCounter++;
                        nodeCounter++;
                        continue;                        
                    }

                    dataSourceTableDataBaseDTO.Headers.Add(columnNode.Name);                    
                    nodeCounter++;
                }

                int columnCounter;
                int effectiveColumnCounter = 0;
                for (int i = 0; i < recordsElementsList.Count; i++)
                {
                    columsToIgnoreCounter = 0;
                    effectiveColumnCounter = 0;
                    string[] arrayValues = new string[dataSourceTableDataBaseDTO.Headers.Count];
                    currentRecordNode = recordsElementsList.Item(i);
                    columnCounter = 0;
                    foreach (XmlNode columnNode in currentRecordNode)
                    {
                        if (columnCounter == columnsToIgnoreList[columsToIgnoreCounter])
                        {
                            columsToIgnoreCounter++;
                            columnCounter++;
                            continue;
                        }

                        arrayValues[effectiveColumnCounter] = columnNode.InnerText;
                        columnCounter++;
                        effectiveColumnCounter++;
                    }                    
                    dataSourceTableDataBaseDTO.Rows.Add(arrayValues);
                }
            }
            catch (XogClientException)
            {
                throw;
            }

            return dataSourceTableDataBaseDTO;
        }

        /// <summary>
        /// Clears clarity sessionId
        /// </summary>
        public void RemoveSessionIdFromCache()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logout of the current SessionID
        /// </summary>
        public void DisposeSession()
        {
            this._xog.DisposeSession();
        }
        #endregion IXogClient functions
    }
}
