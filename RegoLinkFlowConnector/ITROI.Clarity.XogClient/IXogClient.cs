using ITROI.Clarity.XogClient.DTO;
using System.Collections.Generic;
using System.Data;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// Services for XOG cliente interface
    /// </summary>
    public interface IXogClient
    {
        /// <summary>
        /// Get clarity session Id
        /// </summary>
        /// <param name="user">Clarity username</param>
        /// <param name="password">Clarity password</param>
        /// <returns>Session Id for the clarity username</returns>
        string GetSessionId(string user,
                            string password);

        /// <summary>
        /// Assigns the sessionId to the XOG class instance
        /// </summary>
        /// <param name="sessionId">Session Id to assign</param>
        /// <returns>Assigned session Id</returns>
        string SetSessionId(string sessionId);

        /// <summary>
        /// Get the list of queries defined in the PPMVendor
        /// </summary>
        /// <returns>List with tue PPMVendor queries names</returns>
        IList<string> GetQueryList();

        /// <summary>
        /// Get columns information of a query
        /// </summary>
        /// <param name="queryName">Name of the query to extract the columns information</param>
        /// <returns>Datatable with the query columns information</returns>
        DataTable GetQueryColumns(string queryName);

        /// <summary>
        /// Get a filtered query data
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with the columns filter</param>
        /// <param name="fields">Fields of the query to return</param>
        /// <returns>Datatable with the query results</returns>
        DataTable GetQueryData(string queryName, 
                               IDictionary<string, string> filters, 
                               IEnumerable<string> fields);

        /// <summary>
        ///  Executes the XOG request and returns the XOG response as a string
        /// </summary>
        /// <param name="getQueryDataDTO">DTO with the XOG request</param>
        /// <returns>String with the executed XML response</returns>
        string GetQueryDataXML(GetQueryDataDTO GetQueryDataDTO);

        /// <summary>
        /// Get a filtered query data
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with the columns filter</param>
        /// <returns>Datatable with the query results</returns>
        DataTable GetQueryData(string queryName, 
                               IDictionary<string, string> filters);

        /// <summary>
        /// Executes the XML and returns the XOG response as a dataset
        /// </summary>
        /// <param name="xml">XML to execute</param>
        /// <returns>Dataset with the executed XML response</returns>
        DataSet GetDataSetResponseXog(string xml);

        /// <summary>
        /// Executes the XML and returns the XOG response as a string
        /// </summary>
        /// <param name="xml">XML to execute</param>
        /// <returns>String with the executed XML response</returns>
        string GetStrResponseXog(string xml);

        /// <summary>
        /// Executes the write XML
        /// </summary>
        /// <param name="xml">XML to execute</param>
        void WriteXmlXog(string xml);

        /// <summary>
        /// Executes the write XML
        /// </summary>
        /// <param name="xml">XML to execute</param>
        void WriteXmlXogHTTP(string xml);

        /// <summary>
        /// Get XOG response
        /// </summary>
        /// <returns>String with the XOG response</returns>
        string GetResponse();

        /// <summary>
        /// Get XOG request
        /// </summary>
        /// <returns>String with the XOG request</returns>
        string GetXMLRequest();

        /// <summary>
        /// Get query data with sorting
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="filters">Dictionary with query filters {field, value}</param>
        /// <param name="fields">List of the fields to return</param>
        /// <param name="orderBy">Dictionary with the sorting definition {field, sortType}</param>
        /// <param name="sliceDTO">SliceDTO paging object</param>
        /// <returns>Datatable with the query results</returns>
        DataTable GetQueryData(string queryName, 
                               IDictionary<string, string> filters, 
                               IEnumerable<string> fields,
                               IDictionary<string, string> orderBy,
                               SliceDTO sliceDTO);

        /// <summary>
        /// Clears clarity sessionId
        /// </summary>
        void RemoveSessionIdFromCache();

        /// <summary>
        /// Logout of the current SessionID
        /// </summary>
        void DisposeSession();
    }
}