using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ITROI.Clarity.XogClient
{
    public interface IXog
    {
        string GetSessionID(string user, string password);
        string SessionId { get; set; }
        void ExecXmlXog(string xml);
        string Response { get; set; }
        DataSet DataSetResponse { get; set; }
        void ExecXmlXogHTTP(string xml);
        XmlDocument XogResponseDocument  { get; }
        void GetXmlXogResponse(string xml);

        /// <summary>
        /// Logout of the current SessionID
        /// </summary>
        void DisposeSession();
    }
}
