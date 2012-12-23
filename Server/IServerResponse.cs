using System;

namespace logv.http
{
    public interface IServerResponse
    {
        IServerResponse Abort();
        IServerResponse AddCookie(System.Net.Cookie cookie);
        IServerResponse AddHeader(string name, string value);
        void Close();
        System.Text.Encoding ContentEncoding { get; set; }
        long ContentLength64 { get; set; }
        string ContentType { get; set; }
        System.Net.CookieCollection Cookies { get; set; }
        void Dispose();
        bool Equals(object obj);
        int GetHashCode();
        System.Net.WebHeaderCollection Headers { get; set; }
        bool IsCached { get; }
        bool KeepAlive { get; set; }
        System.IO.Stream OutputStream { get; }
        Version ProtocolVersion { get; set; }
        IServerResponse Redirect(string url);
        string RedirectLocation { get; set; }
        bool SendChunked { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
    }
}
