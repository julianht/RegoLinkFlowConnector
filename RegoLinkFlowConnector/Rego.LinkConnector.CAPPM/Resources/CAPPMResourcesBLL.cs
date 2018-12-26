using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Rego.LinkConnector.CAPPM.Resources
{
    /// <summary>
    /// Class with CA PPM resources
    /// </summary>
    public class CAPPMResourcesBLL
    {
        /// <summary>
        /// ResourceManager object
        /// </summary>
        private ResourceManager _resourceManager;

        /// <summary>
        /// Class constructor
        /// </summary>
        public CAPPMResourcesBLL()
        {
            this._resourceManager = new ResourceManager(typeof(CAPPMResources));
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
