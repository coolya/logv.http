using System;

namespace logv.http
{
    /// <summary>
    /// Interface used for all kinds of HTTP responses
    /// </summary>
    public interface IServerResponse
    {
        /// <summary>
        /// Aborts the request
        /// </summary>
        /// <returns>itself</returns>
        IServerResponse Abort();
        /// <summary>
        /// Adds a Cookie
        /// </summary>
        /// <param name="cookie">The cookie to add</param>
        /// <returns>itself wit after the operation was performed</returns>
        IServerResponse AddCookie(System.Net.Cookie cookie);
        /// <summary>
        /// Adds a Header
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <param name="value">Value for the header</param>
        /// <returns>itself</returns>
        IServerResponse AddHeader(string name, string value);
        /// <summary>
        /// Closes the Response
        /// </summary>
        void Close();
        /// <summary>
        /// Property to directly set the Content-Encoding header
        /// </summary>
        System.Text.Encoding ContentEncoding { get; set; }
        /// <summary>
        /// Property to directly set the Content-Length header
        /// </summary>
        long ContentLength64 { get; set; }
        /// <summary>
        /// Property to directly set the Content-Type header
        /// </summary>
        string ContentType { get; set; }
        /// <summary>
        /// Cookie Collection
        /// </summary>
        System.Net.CookieCollection Cookies { get; set; }
        /// <summary>
        /// Frees all the underlying unmanaged resources like streams
        /// </summary>
        void Dispose();
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        bool Equals(object obj);
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();
        /// <summary>
        /// Gets or sets the headers collection.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        System.Net.WebHeaderCollection Headers { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is cached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cached; otherwise, <c>false</c>.
        /// </value>
        bool IsCached { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the connection is [keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [keep alive]; otherwise, <c>false</c>.
        /// </value>
        bool KeepAlive { get; set; }
        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>
        /// The output stream.
        /// </value>
        System.IO.Stream OutputStream { get; }
        /// <summary>
        /// Gets or sets the HTTP protocol version.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        Version ProtocolVersion { get; set; }
        /// <summary>
        /// Redirects the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        IServerResponse Redirect(string url);
        /// <summary>
        /// Gets or sets the redirect location.
        /// </summary>
        /// <value>
        /// The redirect location.
        /// </value>
        string RedirectLocation { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [send chunked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send chunked]; otherwise, <c>false</c>.
        /// </value>
        bool SendChunked { get; set; }
        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>
        /// The status description.
        /// </value>
        string StatusDescription { get; set; }
    }
}
