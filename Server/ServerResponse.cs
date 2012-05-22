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

namespace SimpleHttpServer
{
    public class ServerResponse : IDisposable
    {
        readonly HttpListenerResponse _res;
        readonly Stream _outputStream;

        public ServerResponse(HttpListenerResponse res)
        {
            _res = res;    
            _outputStream = _res.OutputStream;
        }

        public ServerResponse(ServerResponse res) : this(res.InnerResponse) {}

        #region ResponseWrapper

        public ServerResponse Abort()
        { 
            _res.Abort();
            return this;
        }

        public ServerResponse AddHeader(string name, string value)
        {
            _res.AddHeader(name, value);
            return this;
        }

        public ServerResponse AddCookie(Cookie cookie)
        {
            _res.AppendCookie(cookie);
            return this;
        }

        public Encoding ContentEncoding
        {
            get { return _res.ContentEncoding; }
            set { _res.ContentEncoding = value; }
        }

        public long ContentLength64
        {
            get { return _res.ContentLength64; }
            set { _res.ContentLength64 = value; }
        }

        public string ContentType
        {
            get { return _res.ContentType; }
            set { _res.ContentType = value; }
        }

        public CookieCollection Cookies
        {
            get { return _res.Cookies; }
            set { _res.Cookies = value; }
        }

        public bool KeepAlive
        {
            get { return _res.KeepAlive; }
            set { _res.KeepAlive = value; }
        }

        public override bool Equals(object obj)
        {
            var serverResponse = obj as ServerResponse;
            return serverResponse != null ? (serverResponse)._res.Equals(_res) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _res.GetHashCode();
        }

        public WebHeaderCollection Headers
        {
            get { return _res.Headers; }
            set { _res.Headers = value; }
        }

        public string RedirectLocation
        {
            get { return _res.RedirectLocation; }
            set { _res.RedirectLocation = value; }
        }

        public virtual Stream OutputStream
        {
            get { return _outputStream; }
        }

        public Version ProtocolVersion
        {
            get { return _res.ProtocolVersion; }
            set { _res.ProtocolVersion = value; }
        }

        public ServerResponse Redirect(string url)
        {
            _res.Redirect(url);
            return this;
        }

        public bool SendChunked
        {
            get { return _res.SendChunked; }
            set { _res.SendChunked = value; }
        }

        public int StatusCode
        {
            get { return _res.StatusCode; }
            set { _res.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _res.StatusDescription; }
            set { _res.StatusDescription = value; }
        }


        #endregion

        protected HttpListenerResponse InnerResponse { get { return _res; } }

        public void Dispose()
        {
            Close();
        }

        public virtual void Close()
        {
            _outputStream.Close();
            _res.Close();
        }
    }
}
