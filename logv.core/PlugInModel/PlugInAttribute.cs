
/*
     Copyright 2012 Kolja Dummann <k.dummann@gmail.com>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
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
