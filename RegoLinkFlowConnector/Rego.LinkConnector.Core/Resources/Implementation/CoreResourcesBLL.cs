using Rego.LinkConnector.Core.Resources.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.Core.Resources.Implementation
{
    /// <summary>
    /// Class with resources files logic
    /// </summary>
    public class CoreResourcesBLL : ICoreResourcesBLL
    {
        /// <summary>
        /// ResourceManager object
        /// </summary>
        private ResourceManager _resourceManager;

        /// <summary>
        /// Class constructor
        /// </summary>
        public CoreResourcesBLL()
        {
            this._resourceManager = new ResourceManager(typeof(CoreResources));
        }

        /// <summary>
        /// Gets the resource item by name
        /// </summary>
        /// <param name="name">Name of the item to recover</param>
        /// <returns>Resource value associated to the name</returns>
        public string GetResource(string name)
        {
            string value = this._resourceManager.GetString(name);

            return value != null ? value : "";
        }
    }
}
