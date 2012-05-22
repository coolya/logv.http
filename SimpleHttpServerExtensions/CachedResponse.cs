using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleHttpServer;

namespace SimpleHttpServer
{
    sealed class CachedResponse : ServerResponse, IDisposable
    {
        ServerResponse res;
        MemoryStream bufferStream = new MemoryStream();

        public CachedResponse(ServerResponse res) : base (res)
        {
            this.res = res;
            cached = true;
        }

        public override System.IO.Stream OutputStream
        {
            get
            {
                return bufferStream;
            }
        }

        public override long ContentLength64
        {
            get
            {
                return base.ContentLength64;
            }
            set
            {
                throw new InvalidOperationException("Content-Length will be set when the request is closed!");
            }
        }

        public override void Close()
        {
            Commit();
        }

        void Commit()
        {
            res.ContentLength64 = bufferStream.Length;
            bufferStream.Position = 0;
            var copier = new AsyncStreamCopier(bufferStream, res.OutputStream);

            copier.Completed += (s, e) =>
            {
                bufferStream.Close();
                res.Close();
            };
        }
    }
}
