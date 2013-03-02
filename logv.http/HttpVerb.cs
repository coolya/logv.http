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
namespace logv.http
{
    /// <summary>
    /// Enumeration describing the HTTP Verbs
    /// </summary>
    public enum HttpVerb
    {
        /// <summary>
        /// Get
        /// </summary>
        Get,
        /// <summary>
        /// Post
        /// </summary>
        Post,
        /// <summary>
        /// Put
        /// </summary>
        Put,
        /// <summary>
        /// Delete
        /// </summary>
        Delete,
        /// <summary>
        /// Head
        /// </summary>
        Head,
        /// <summary>
        /// Patch
        /// </summary>
        Patch
    }
}
