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
using System.Text;
using System.Net;
using System.IO;

namespace logv.http
{
    /// <summary>
    /// The default implementation of IServerResponse that wraps a HttpListenerResponse
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ServerResponse : IDisposable, IServerResponse
    {
        readonly HttpListenerResponse _res;
        readonly Stream _outputStream;
        /// <summary>
        /// The cached
        /// </summary>
        protected bool cached = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResponse" /> class.
        /// </summary>
        /// <param name="res">The res.</param>
        public ServerResponse(HttpListenerResponse res)
        {
            _res = res;    
            _outputStream = _res.OutputStream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResponse" /> class.
        /// </summary>
        /// <param name="res">The res.</param>
        public ServerResponse(IServerResponse res) : this((res as ServerResponse).InnerResponse) { }

        
        #region ResponseWrapper

        /// <summary>
        /// Aborts the request
        /// </summary>
        /// <returns>
        /// itself
        /// </returns>
        public IServerResponse Abort()
        { 
            _res.Abort();
            return this;
        }

        /// <summary>
        /// Adds a Header
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <param name="value">Value for the header</param>
        /// <returns>
        /// itself
        /// </returns>
        public IServerResponse AddHeader(string name, string value)
        {
            _res.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// Adds a Cookie
        /// </summary>
        /// <param name="cookie">The cookie to add</param>
        /// <returns>
        /// itself wit after the operation was performed
        /// </returns>
        public IServerResponse AddCookie(Cookie cookie)
        {
            _res.AppendCookie(cookie);
            return this;
        }

        /// <summary>
        /// Property to directly set the Content-Encoding header
        /// </summary>
        public Encoding ContentEncoding
        {
            get { return _res.ContentEncoding; }
            set { _res.ContentEncoding = value; }
        }

        /// <summary>
        /// Property to directly set the Content-Length header
        /// </summary>
        public virtual long ContentLength64
        {
            get { return _res.ContentLength64; }
            set { _res.ContentLength64 = value; }
        }

        /// <summary>
        /// Property to directly set the Content-Type header
        /// </summary>
        public string ContentType
        {
            get { return _res.ContentType; }
            set { _res.ContentType = value; }
        }

        /// <summary>
        /// Cookie Collection
        /// </summary>
        public CookieCollection Cookies
        {
            get { return _res.Cookies; }
            set { _res.Cookies = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is [keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [keep alive]; otherwise, <c>false</c>.
        /// </value>
        public bool KeepAlive
        {
            get { return _res.KeepAlive; }
            set { _res.KeepAlive = value; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var serverResponse = obj as ServerResponse;
            return serverResponse != null ? (serverResponse)._res.Equals(_res) : base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _res.GetHashCode();
        }

        /// <summary>
        /// Gets or sets the headers collection.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public WebHeaderCollection Headers
        {
            get { return _res.Headers; }
            set { _res.Headers = value; }
        }

        /// <summary>
        /// Gets or sets the redirect location.
        /// </summary>
        /// <value>
        /// The redirect location.
        /// </value>
        public string RedirectLocation
        {
            get { return _res.RedirectLocation; }
            set { _res.RedirectLocation = value; }
        }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>
        /// The output stream.
        /// </value>
        public virtual Stream OutputStream
        {
            get { return _outputStream; }
        }

        /// <summary>
        /// Gets or sets the HTTP protocol version.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public Version ProtocolVersion
        {
            get { return _res.ProtocolVersion; }
            set { _res.ProtocolVersion = value; }
        }

        /// <summary>
        /// Redirects the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IServerResponse Redirect(string url)
        {
            _res.Redirect(url);
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send chunked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send chunked]; otherwise, <c>false</c>.
        /// </value>
        public bool SendChunked
        {
            get { return _res.SendChunked; }
            set { _res.SendChunked = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode
        {
            get { return _res.StatusCode; }
            set { _res.StatusCode = value; }
        }

        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>
        /// The status description.
        /// </value>
        public string StatusDescription
        {
            get { return _res.StatusDescription; }
            set { _res.StatusDescription = value; }
        }


        #endregion

        /// <summary>
        /// Gets the inner response.
        /// </summary>
        /// <value>
        /// The inner response.
        /// </value>
        protected HttpListenerResponse InnerResponse { get { return _res; } }

        /// <summary>
        /// Gets a value indicating whether this instance is cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cached; otherwise, <c>false</c>.
        /// </value>
        public bool IsCached { get { return cached; } }

        /// <summary>
        /// Disposes the native unmanaged resources 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes the Response
        /// </summary>
        public virtual void Close()
        {
            _outputStream.Flush();            
            _res.Close();
        }
    }
}
