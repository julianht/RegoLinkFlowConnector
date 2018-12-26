using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITROI.Clarity.XogClient.Contracts
{
    public interface IDataSourceEndPointDTO
    {
        string APIKey { get; set; }
        string EndPointURL { get; set; }

        /// <summary>
        /// Get if the endpoint it's defined
        /// </summary>
        bool IsDefined { get; }
    }
}
