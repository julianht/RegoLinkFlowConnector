using ITROI.Clarity.XogClient.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITROI.Clarity.XogClient
{
    public class XogFactory
    {
        public static IXog GetXogImplementation(string url)
        {
            string ecBossXOGSOAP = ConfigurationManager.AppSettings["ecBossXOGSOAP"];

            if (string.IsNullOrEmpty(ecBossXOGSOAP) ||
                ecBossXOGSOAP == "1")
            {
                return new Xog(url);
            }

            return new XogHttpWebRequest(url);
        }

        public static IXog GetXogImplementation(IDataSourceEndPointDTO dataSourceEndPointDTO)
        {
            return new XogHttpWebRequest(dataSourceEndPointDTO);
        }
    }
}
