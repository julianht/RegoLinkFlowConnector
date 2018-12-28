using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Log.Contracts
{
    /// <summary>
    /// Contract for log definition
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes a error message in the log
        /// </summary>
        /// <param name="message">Error message</param>
        void WriteLogError(string message);

        /// <summary>
        /// Writes a info message in the log
        /// </summary>
        /// <param name="message">Info message</param>
        void WriteLogInfo(string message);

        /// <summary>
        /// Writes a warning message in the log
        /// </summary>
        /// <param name="message">Warning message</param>
        void WriteLogWarning(string message);
    }
}
