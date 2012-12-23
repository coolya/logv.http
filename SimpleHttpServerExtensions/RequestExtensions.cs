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
using System.IO;
using System.Net;
using System.Text;

namespace logv.http
{
    /// <summary>
    /// Helper class with HttpListenerRequest Extension methods
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Reads the content the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>The content as string</returns>
        public static string Content(this HttpListenerRequest req)
        {
            string result = string.Empty;

            if (req.ContentLength64 > 0)
            {
                var bytez = new byte[req.ContentLength64];
                var reader = new StreamReader(req.InputStream);
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// Gets the encoding. Currently a placeholder that does only return UTF8
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
        public static Encoding GetEncoding(this HttpListenerRequest req)
        {
            //we only support UTF encoded bodies
            return Encoding.UTF8;
        }

        /// <summary>
        /// Gets the accept charset.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
        public static Encoding GetAcceptCharset(this HttpListenerRequest req)
        {
            var value = req.Headers.Get("Accept-Charset");

            if (string.IsNullOrEmpty(value))
                return Encoding.UTF8;

            return GetEncodingFromHeader(value);
        }

        /// <summary>
        /// Gets a proxy Response according to the Accept-Encoding header
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="res">The res.</param>
        /// <returns>A proxy request that compresses the payload</returns>
        public static IServerResponse GetCompressedResponse(this HttpListenerRequest req, IServerResponse res)
        {
            var enc = req.GetAcceptEncoding();

            switch(enc)
            {
                case EncodingType.Deflate:
                    return new DeflateResponse(res);
                case EncodingType.Gzip:
                    return new GzipResponse(res);
                default:
                    return res;
            }
        }

        /// <summary>
        /// Gets the accept encoding from the header
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
        public static EncodingType GetAcceptEncoding(this HttpListenerRequest req)
        {
            EncodingType ret = EncodingType.Plain;
            var value = req.Headers.Get("Accept-Encoding");

            if(!string.IsNullOrEmpty(value))
            {

            if(value.Contains("gzip"))
                ret = EncodingType.Gzip;
            else if (value.Contains("deflate"))
                ret = EncodingType.Deflate;
            else
                ret = EncodingType.Plain;
            }

            return ret;
        }

        private static Encoding GetEncodingFromHeader(string value)
        {
            Encoding ret = null;
            if(value.Contains("UTF-8"))
            {
                ret = Encoding.UTF8;
            } else 
            {
                var splits = value.Split(',');

                foreach (var item in splits)
                {
                        try 
                        {	        
                            ret = Encoding.GetEncoding(splits[0]);
                            continue;
                        }
                        catch (Exception)
                        {	
                        
                        }
                }
             
                if(ret == null)
                    ret = Encoding.UTF8;            
            }

            return ret;
        }

        /// <summary>
        /// Dumps the header.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
        public static string DumpHeader(this HttpListenerRequest req)
        {
            return string.Format("->->->->-> REQUEST <-<-<-<-<-" + Environment.NewLine +
                                   "->->->->-> Url: {0}" + Environment.NewLine +
                                   "->->->->-> From: {1}" + Environment.NewLine +
                                   "->->->->-> Method: {2}" + Environment.NewLine +
                                   "->->->->-> Header: {3}", req.Url, req.RemoteEndPoint.Address, req.HttpMethod, req.Headers.ToString());
        }

        /// <summary>
        /// Dumps the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
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
