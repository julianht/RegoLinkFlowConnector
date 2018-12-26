using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Resources.Contracts
{
    /// <summary>
    /// Interface with resources logic
    /// </summary>
    public interface ICoreResourcesBLL
    {
        /// <summary>
        /// Gets the resource item by name
        /// </summary>
        /// <param name="name">Name of the item to recover</param>
        /// <returns>Resource value associated to the name</returns>
        string GetResource(string name);
    }
}
