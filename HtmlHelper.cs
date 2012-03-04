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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SimpleHttpServer
{
    public static class HtmlHelper
    {
        public static string PackIntoHtml(string body)
        {
            return string.Format("<HTML><BODY>{0}</BODY></HTML>", body);
        }

        public static string GetJson(object obj)
        {
            string result = null;

            var data = new MemoryStream();

            try
            {
                var ser = new DataContractJsonSerializer(obj.GetType());

                ser.WriteObject(data, obj);

                result = Encoding.UTF8.GetString(data.ToArray());
            }
            finally
            {
                data.Close();
            }

            return result;
        }

        public static void WriteJson(this Stream stream, object obj)
        {
            var ser = new DataContractJsonSerializer(obj.GetType());

            ser.WriteObject(stream, obj);
        }


        public static T GetObject<T>(string json)
        {
            var data = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var ser = new DataContractJsonSerializer(typeof (T));

            return (T) ser.ReadObject(data);
        }

        public static T GetObject<T>(this Stream stream)
        {
            var ser = new DataContractJsonSerializer(typeof(T));

            return (T)ser.ReadObject(stream);
        }
    }
}
