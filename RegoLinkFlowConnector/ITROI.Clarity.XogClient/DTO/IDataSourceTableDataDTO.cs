using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.DTO
{
    public interface IDataSourceTableDataDTO
    {
        IList<string> Headers { get; set; }
        IList<string[]> Rows { get; set; }
    }
}
