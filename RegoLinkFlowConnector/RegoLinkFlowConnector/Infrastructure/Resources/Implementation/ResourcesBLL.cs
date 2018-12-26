using RegoLinkFlowConnector.Infrastructure.Resources.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace RegoLinkFlowConnector.Infrastructure.Resources.Implementation
{
    /// <summary>
    /// Class with resources files logic
    /// </summary>
    public class ResourcesBLL : IResourcesBLL
    {
        /// <summary>
        /// ResourceManager object
        /// </summary>
        private ResourceManager _resourceManager;

        /// <summary>
        /// Class constructor
        /// </summary>
        public ResourcesBLL()
        {
            this._resourceManager = new ResourceManager(typeof(Resources));
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