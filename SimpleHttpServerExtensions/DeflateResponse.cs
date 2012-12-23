using System.IO;
using System.IO.Compression;

namespace logv.http
{
    /// <summary>
    /// A deflate compressed response
    /// </summary>
    public class DeflateResponse : CachedResponse
    {
        private Stream deflateStream;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeflateResponse" /> class.
        /// </summary>
        /// <param name="res">The res.</param>
        public DeflateResponse(IServerResponse res) : base (res)
        {
            Headers.Add("Content-Encoding", "deflate");
            deflateStream = new DeflateStream(base.OutputStream, CompressionMode.Compress, true);
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
                return deflateStream;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            deflateStream.Close();
            base.Close();
        }
    }
}
