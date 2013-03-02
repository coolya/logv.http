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

using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;

namespace logv.http
{
    /// <summary>
    /// Helper class with html tools
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// Packs the into HTML body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static string PackIntoHtmlBody(string body)
        {
            return string.Format("<HTML><BODY>{0}</BODY></HTML>", body);
        }

        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetJson(object obj)
        {
            //string result = null;
            var converter = new JsonSerializer();

            JsonSerializer jsonSerializer = JsonSerializer.Create(null);

            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.None;

                jsonSerializer.Serialize(jsonWriter, obj);
            }

            return sw.ToString();


//            return result;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T GetObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
