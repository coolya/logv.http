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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleHttpServer;

namespace SimpleHttpServer
{
    sealed class CachedResponse : ServerResponse, IDisposable, IServerResponse
    {
        IServerResponse res;
        MemoryStream bufferStream = new MemoryStream();

        public CachedResponse(IServerResponse res)
            : base(res)
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
