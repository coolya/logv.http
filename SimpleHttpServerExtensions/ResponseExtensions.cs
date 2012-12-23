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
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace logv.http
{
    /// <summary>
    /// Helper class for IServer response extension methods
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Writes the given object to the response as JSON
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>itself</returns>
        public static IServerResponse WriteAsJson(this IServerResponse res, object obj)
        {
            res.Headers.Add("Content-Type", "application/json");
            return res.Write(HtmlHelper.GetJson(obj));            
        }

        /// <summary>
        /// Writes the given object to the response as JSON
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="charset">The charset.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>itself</returns>
        public static IServerResponse WriteAsJson(this IServerResponse res, object obj, Encoding charset, EncodingType encoding)
        {
            res.Headers.Add("Content-Type", "application/json");
            return res.Write(HtmlHelper.GetJson(obj), charset, encoding);            
        }

        /// <summary>
        /// Serves the given file to the client and closes the request when done. 
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="System.ArgumentException">File does not exist</exception>
        public static void ServeFile(this IServerResponse res, string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("File does not exist");

            if (!res.IsCached)
            {
                res.ContentLength64 = new FileInfo(path).Length;
            }

            var fileInfo = new FileInfo(path);
            res.ContentLength64 = fileInfo.Length;

            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var copier = new AsyncStreamCopier(stream, res.OutputStream);

            copier.Completed += (s, e) =>
            {
                e.InputStream.Close();
                res.Close();
            };

            copier.Copy();
        }
        /// <summary>
        /// Serves the given file to the client and closes the request when done. 
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="path">The path.</param>
        /// <param name="mimeType">Type of the MIME type.</param>
        public static void ServeFile(this IServerResponse res, string path, string mimeType)
        {
           res.Headers.Add("Content-Type", mimeType);
           res.ServeFile(path); 
        }



        /// <summary>
        /// Writes the given string to the response
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="data">The data.</param>
        /// <returns>itself</returns>
        public static IServerResponse Write(this IServerResponse res, string data)
        {
            return res.Write(data, Encoding.UTF8, EncodingType.Plain);
        }

        /// <summary>
        /// Writes the given string to the response
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="data">The data.</param>
        /// <param name="charset">The charset.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>itself</returns>
        public static IServerResponse Write(this IServerResponse res, string data, Encoding charset, EncodingType encoding)
        {
            var bytez = charset.GetBytes(data);
            return res.Write(encoding, bytez);            
        }

        /// <summary>
        /// Response with the status code:
        /// 201 Created
        /// The request has been fulfilled and resulted in a new resource being created.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="newLocation">The new location of the created resource</param>
        /// <returns></returns>
        public static IServerResponse Response201(this IServerResponse res, Uri newLocation)
        {
            res.StatusCode = 201;
            res.Headers.Add("Location", newLocation.ToString());
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 202 Accepted
        /// The request has been accepted for processing, but the processing has not been completed.
        /// The request might or might not eventually be acted upon,
        /// as it might be disallowed when processing actually takes place. You might add additional data
        /// to the response body for the client to check the state of the operation.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response202(this IServerResponse res)
        {
            res.StatusCode = 202;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 204 No Content
        /// The server successfully processed the request, but is not returning any content
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response204(this IServerResponse res)
        {
            res.StatusCode = 204;            
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 204 No Content
        /// The server successfully processed the request, but is not returning any content
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="additinalHeaders">The additional headers.</param>
        public static void Response204(this IServerResponse res, System.Collections.Specialized.NameValueCollection additinalHeaders)
        {
            res.Headers.Add(additinalHeaders);
            res.Response204();
        }

        /// <summary>
        /// Response with the status code:
        /// 301 Moved Permanently
        /// This and all future requests should be directed to the given URI.
        /// The client normally will issue a GET request to that url not matter what
        /// the original request was.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="newLocation">The new location.</param>
        public static void Response301(this IServerResponse res, Uri newLocation)
        {
            res.StatusCode = 301;
            res.Headers.Add("Location", newLocation.ToString());
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 303 See Other
        /// The response to the request can be found under another URI using a GET method. 
        /// When received in response to a POST (or PUT/DELETE),
        /// it should be assumed that the server has received the data 
        /// and the redirect should be issued with a separate GET message.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="other">The other location.</param>
        public static void Response303(this IServerResponse res, Uri other)
        {
            res.StatusCode = 303;
            res.Headers.Add("Location", other.ToString());
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 304 Not Modified
        /// Indicates the resource has not been modified since last requested.
        /// Typically, the HTTP client provides a header like the If-Modified-Since header
        /// to provide a time against which to compare. 
        /// Using this saves bandwidth and reprocessing on both the server and client,
        /// as only the header data must be sent and received in comparison to the entirety
        /// of the page being re-processed by the server, 
        /// then sent again using more bandwidth of the server and client.
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response304(this IServerResponse res)
        {
            res.StatusCode = 304;
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 307 Temporary Redirect
        /// In this case, the request should be repeated with another URI; 
        /// however, future requests should still use the original URI.
        /// In contrast to how 302 was historically implemented, 
        /// the request method is not allowed to be changed when reissuing the original request.
        /// For instance, a POST request repeated using another POST request.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="newLocation">The new location.</param>
        public static void Response307(this IServerResponse res, Uri newLocation)
        {
            res.StatusCode = 307;
            res.Headers.Add("Location", newLocation.ToString());
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 400 Bad Request
        /// The request cannot be fulfilled due to bad syntax.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response400(this IServerResponse res)
        {
            res.StatusCode = 400;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 401 Unauthorized
        /// Similar to 403 Forbidden, but specifically for use when authentication is required and
        /// has failed or has not yet been provided. The response must include a WWW-Authenticate header field
        /// containing a challenge applicable to the requested resource. 
        /// See Basic access authentication and Digest access authentication.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response401(this IServerResponse res)
        {
            res.StatusCode = 401;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 403 Forbidden
        /// The request was a valid request, but the server is refusing to respond to it.
        /// Unlike a 401 Unauthorized response, authenticating will make no difference.
        /// On servers where authentication is required, 
        /// this commonly means that the provided credentials were successfully authenticated 
        /// but that the credentials still do not grant the client permission to access the resource 
        /// (e.g. a recognized user attempting to access restricted content).
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response403(this IServerResponse res)
        {
            res.StatusCode = 403;
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 404 Not Found
        /// The requested resource could not be found but may be available again in the future.
        /// Subsequent requests by the client are permissible.
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response404(this IServerResponse res)
        {
            res.StatusCode = 404;
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 405 Method Not Allowed
        /// A request was made of a resource using a request method not supported by that resource;
        /// for example, using GET on a form which requires data to be presented via POST, 
        /// or using PUT on a read-only resource.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="allowedVerbs">The allowed verbs.</param>
        public static void Response405(this IServerResponse res, params HttpVerb[] allowedVerbs)
        {
            res.StatusCode = 405;
            var data = allowedVerbs.Aggregate("", (str, item) => str += item.ToString() + ",");
            res.Headers.Add("Allow", data.TrimEnd(','));
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 409 Conflict
        /// Indicates that the request could not be processed because of conflict in the request,
        /// such as an edit conflict.
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response409(this IServerResponse res)
        {
            res.StatusCode = 409;
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 418 I'm a teapot (RFC 2324)
        /// This code was defined in 1998 as one of the traditional IETF April Fools' jokes, 
        /// in RFC 2324, Hyper Text Coffee Pot Control Protocol, and is not expected to be implemented by
        /// actual HTTP servers.
        /// </summary>
        /// <param name="res">The response to the client</param>
        public static void Response418(this IServerResponse res)
        {
            res.StatusCode = 418;
            res.Close();
        }

        /// <summary>
        /// Response with the status code:
        /// 451 Unavailable For Legal Reasons (Internet draft)
        /// Defined in the internet draft "A New HTTP Status Code for Legally-restricted Resources".
        /// Intended to be used when resource access is denied for legal reasons,
        /// e.g. censorship or government-mandated blocked access. 
        /// A reference to the 1953 dystopian novel Fahrenheit 451, where books are outlawed.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response451(this IServerResponse res)
        {
            res.StatusCode = 451;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 500 Internal Server Error
        /// A generic error message, given when no more specific message is suitable.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response500(this IServerResponse res)
        {
            res.StatusCode = 500;
            return res;
        }


        /// <summary>
        /// Response with the status code:
        /// 500 Internal Server Error
        /// A generic error message, given when no more specific message is suitable.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static IServerResponse Response500(this IServerResponse res, Exception ex)
        {
            res.StatusCode = 500;
            res.StatusDescription = ex.Message.Length < 513 ? ex.Message.Replace("\r\n", "") : string.Empty;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 503 Service Unavailable
        /// The server is currently unavailable (because it is overloaded or down for maintenance).
        /// Generally, this is a temporary state.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static IServerResponse Response503(this IServerResponse res, Exception ex)
        {
            res.StatusCode = 503;
            res.StatusDescription = ex.Message.Length < 513 ? ex.Message.Replace("\r\n", "") : string.Empty;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 503 Service Unavailable
        /// The server is currently unavailable (because it is overloaded or down for maintenance).
        /// Generally, this is a temporary state.
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response503(this IServerResponse res)
        {
            res.StatusCode = 503;
            return res;
        }

        /// <summary>
        /// Response with the status code:
        /// 507 Insufficient Storage
        /// The server is unable to store the representation needed to complete the request
        /// </summary>
        /// <param name="res">The response to the client</param>
        /// <returns></returns>
        public static IServerResponse Response507(this IServerResponse res)
        {
            res.StatusCode = 507;
            return res;
        }

        /// <summary>
        /// Writes the specified byte data to the response
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static IServerResponse Write(this IServerResponse res, EncodingType type, byte[] data)
        {    

            switch (type)
            {
                case EncodingType.Deflate:
                    {
                        res.Headers.Add("Content-Encoding", "deflate");

                        if (!res.IsCached)
                            res = res.GetCachedResponse();

                        var stream = new DeflateStream(res.OutputStream, CompressionMode.Compress);
                        
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                    break;
                case EncodingType.Gzip:
                    {
                        res.Headers.Add("Content-Encoding", "gzip");

                        if (!res.IsCached)
                            res = res.GetCachedResponse();

                        var stream = new GZipStream(res.OutputStream, CompressionMode.Compress);
                        
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                    break;
                case EncodingType.Plain:

                    if (!res.IsCached)
                        res.ContentLength64 = data.Length;

                    res.OutputStream.Write(data, 0, data.Length);
                    break;
            }
 
            return res;
        }

        /// <summary>
        /// Gets the cached response that caches the data into a MemoryStream
        /// </summary>
        /// <param name="res">The res.</param>
        /// <returns></returns>
        public static IServerResponse GetCachedResponse(this IServerResponse res)
        {
            return new CachedResponse(res);
        }

        /// <summary>
        /// Appends a "public" to the Cache-Control header to allow public cache to cache the resource
        /// </summary>
        /// <param name="res">The res.</param>
        /// <returns></returns>
        public static IServerResponse CachePublic(this IServerResponse res)
        {
            var header = res.Headers.Get("Cache-Control");

            if (!string.IsNullOrEmpty(header))
            {
                header = "public, " + header;
            }
            else
            {
                header = "public";
            }
            res.Headers["Cache-Control"] = header;
            

            return res;
        }

        /// <summary>
        /// Appends a "private" to the Cache-Control header to deny caching on shared caches.
        /// </summary>
        /// <param name="res">The res.</param>
        /// <returns></returns>
        public static IServerResponse CachePrivate(this IServerResponse res)
        {
            var header = res.Headers.Get("Cache-Control");

            if (!string.IsNullOrEmpty(header))
            {
                header = "private, " + header;
            }
            else
            {
                header = "private";
            }
            res.Headers["Cache-Control"] = header;

            return res;
        }

        /// <summary>
        /// Set the Cache-Control header to no-cache to enforce revalidation
        /// </summary>
        /// <param name="res">The res.</param>
        /// <returns></returns>
        public static IServerResponse NoCache(this IServerResponse res)
        {
            var header = res.Headers.Get("Cache-Control");

            if (!string.IsNullOrEmpty(header))
            {
                header = header + ", no-cache";
            }
            else
            {
                header = "no-cache";
            }

            res.Headers["Cache-Control"] = header;

            return res;
        }

        /// <summary>
        /// Sets the max-age part of the Cache-Control header
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        public static IServerResponse MaxAge(this IServerResponse res, int seconds)
        {
            var header = res.Headers.Get("Cache-Control");

            if (!string.IsNullOrEmpty(header))
            {
                header = header +  string.Format(", max-age={0}", seconds);
            }
            else
            {
                header =  string.Format("max-age={0}", seconds);
            }
            res.Headers["Cache-Control"] = header;

            return res;
        }

        /// <summary>
        /// Set the ETag header.
        /// </summary>
        /// <param name="res">The res.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IServerResponse ETag(this IServerResponse res, string tag)
        {
            res.AddHeader("ETag", tag);

            return res;
        }

    }
}
