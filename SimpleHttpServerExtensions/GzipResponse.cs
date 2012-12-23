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
