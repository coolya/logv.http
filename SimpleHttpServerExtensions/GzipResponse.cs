using System.IO;
using System.IO.Compression;

namespace logv.http
{
    /// <summary>
    /// A GZip compressed response
    /// </summary>
    public class GzipResponse : CachedResponse
    {
        private Stream gzipStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipResponse" /> class.
        /// </summary>
        /// <param name="res">The res.</param>
        public GzipResponse(IServerResponse res) : base(res)
        {
            Headers.Add("Content-Encoding", "gzip");
            gzipStream = new GZipStream(base.OutputStream, CompressionMode.Compress, true);
        }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>
        /// The output stream.
        /// </value>
        public override Stream OutputStream
        {
            get
            {
                return gzipStream;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            gzipStream.Close();
            base.Close();
        }

    }
}
