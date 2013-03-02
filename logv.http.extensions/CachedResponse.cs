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
using System.IO;

namespace logv.http
{
    /// <summary>
    /// A response the caches the output stream into a MemoryStream and sends it to client async when it is closed 
    /// </summary>
    public class CachedResponse : ServerResponse, IDisposable, IServerResponse
    {
        IServerResponse res;
        MemoryStream bufferStream = new MemoryStream();

        internal CachedResponse(IServerResponse res)
            : base(res)
        {
            this.res = res;
            cached = true;
        }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>
        /// The output stream.
        /// </value>
        public override System.IO.Stream OutputStream
        {
            get
            {
                return bufferStream;
            }
        }

        /// <summary>
        /// Property to directly set the Content-Length header
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Content-Length will be set when the request is closed!</exception>
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

        /// <summary>
        /// Closes the Response
        /// </summary>
        public override void Close()
        {
            Commit();
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        void Commit()
        {
            res.ContentLength64 = bufferStream.Length;
            bufferStream.Position = 0;
            var copier = new AsyncStreamCopier(bufferStream, res.OutputStream);

            copier.Completed += (s, e) =>
            {
                bufferStream.Flush();
                bufferStream.Close();
                res.Close();
            };

            copier.Copy();
        }
    }
}
