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
