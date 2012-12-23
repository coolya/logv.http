using System.IO;
using System.IO.Compression;

namespace logv.http
{
    public class DeflateResponse : CachedResponse
    {
        private Stream deflateStream;
        public DeflateResponse(IServerResponse res) : base (res)
        {
            Headers.Add("Content-Encoding", "deflate");
            deflateStream = new DeflateStream(base.OutputStream, CompressionMode.Compress, true);
        }

        public override Stream OutputStream
        {
            get
            {
                return deflateStream;
            }
        }

        public override void Close()
        {
            deflateStream.Close();
            base.Close();
        }
    }
}
