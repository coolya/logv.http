using SimpleHttpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SimpleHttpServer
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
