using SimpleHttpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SimpleHttpServer
{
    public class GzipResponse : CachedResponse
    {
        private Stream gzipStream;

        public GzipResponse(IServerResponse res) : base(res)
        {
            Headers.Add("Content-Encoding", "gzip");
            gzipStream = new GZipStream(base.OutputStream, CompressionMode.Compress, true);
        }

        public override Stream OutputStream
        {
            get
            {
                return gzipStream;
            }
        }

        public override void Close()
        {
            gzipStream.Close();
            base.Close();
        }

    }
}
