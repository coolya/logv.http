using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logv.core.PlugInModel
{
    /// <summary>
    /// This attribute is used by the hosting components to find and activate plugins
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlugInAttribute : Attribute
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Create a new instance of <see cref="T:logv.core.PlugInModel.PlugInAttribute"/>
        /// </summary>
        /// <param name="name">The name of the plugin</param>
        public PlugInAttribute(string name)
        {
            Name = name;
        }
    }
}
