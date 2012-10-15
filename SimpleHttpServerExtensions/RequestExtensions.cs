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
using System.Net;
using System.Text;

namespace SimpleHttpServer
{
    public static class RequestExtensions
    {
        public static string Content(this HttpListenerRequest req)
        {
            string result = string.Empty;

            if (req.ContentLength64 > 0)
            {
                var bytez = new byte[req.ContentLength64];
                req.InputStream.Read(bytez, 0, (int)req.ContentLength64);
                result = req.GetEncoding().GetString(bytez);
            }
            return result;
        }

        public static Encoding GetEncoding(this HttpListenerRequest req)
        {
            //todo check Content-Encoding an return the correct encoding
            return Encoding.UTF8;
        }

        public static string DumpHeader(this HttpListenerRequest req)
        {
            return string.Format("->->->->-> REQUEST <-<-<-<-<-" + Environment.NewLine +
                                   "->->->->-> Url: {0}" + Environment.NewLine +
                                   "->->->->-> From: {1}" + Environment.NewLine +
                                   "->->->->-> Method: {2}" + Environment.NewLine +
                                   "->->->->-> Header: {3}", req.Url, req.RemoteEndPoint.Address, req.HttpMethod, req.Headers.ToString());
        }

        public static string Dump(this HttpListenerRequest req)
        {
            return string.Format("->->->->-> REQUEST <-<-<-<-<-" + Environment.NewLine +
                                   "->->->->-> Url: {0}" + Environment.NewLine +
                                   "->->->->-> From: {1}" + Environment.NewLine +
                                   "->->->->-> Method: {2}" + Environment.NewLine +
                                   "->->->->-> Header: {3}" + Environment.NewLine +
                                   "->->->->-> Content: {4}"
                                   , req.Url, req.RemoteEndPoint.Address, req.HttpMethod, req.Headers.ToString(), req.Content());
        }
    }
}
