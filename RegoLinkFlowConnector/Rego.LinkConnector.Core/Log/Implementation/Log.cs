using log4net;
using Rego.LinkConnector.Core.Log.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Log.Implementation
{
    /// <summary>
    /// Cklass with log methods
    /// </summary>
    public class Log : Contracts.ILog
    {
        /// <summary>
        /// log4net logger instance
        /// </summary>
        private static readonly log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ILog implementation
        /// <summary>
        /// Writes a error message in the log
        /// </summary>
        /// <param name="message">Error message</param>
        public void WriteLogError(string message)
        {
            _logger.Error(message);
        }

        /// <summary>
        /// Writes a info message in the log
        /// </summary>
        /// <param name="message">Info message</param>
        public void WriteLogInfo(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// Writes a warning message in the log
        /// </summary>
        /// <param name="message">Warning message</param>
        public void WriteLogWarning(string message)
        {
            _logger.Warn(message);
        }
        #endregion
    }
}
