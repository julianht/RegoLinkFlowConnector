using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ITROI.Clarity.XogClient.Exceptions;



namespace ITROI.Clarity.XogClient.DTO
{
    public class DataSourceTableDataBaseDTO : IDataSourceTableDataDTO
    {
        public DataSourceTableDataBaseDTO()
        {
            Headers = new List<string>();
            Rows = new List<string[]>();
        }

        public IList<string> Headers { get; set; }
        public IList<string[]> Rows { get; set; }

        
    }
}
