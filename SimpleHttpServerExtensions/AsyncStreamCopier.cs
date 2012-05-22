using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleHttpServer
{
    /// <summary>
    /// Copies data from one stream into another using the async pattern. Copies are done in 4k blocks.
    /// </summary>
    public class AsyncStreamCopier
    {
        readonly Stream _input;
        readonly Stream _output;

        byte[] buffer = new byte[4096];

        /// <summary>
        /// Raised when all data is copied
        /// </summary>
        public event EventHandler Completed;

        public AsyncStreamCopier(Stream input, Stream output)
        {
            _input = input;
            _output = output;
        }

        /// <summary>
        /// Starts the copying
        /// </summary>
        public void Copy()
        {
            GetData();
        }

        void GetData()
        {
            _input.BeginRead(buffer, 0, buffer.Length, ReadComplete, null);
        }

        void ReadComplete(IAsyncResult result)
        {

            int bytes = _input.EndRead(result);

            if (bytes == 0)
            {
                RaiseComplete();
                return;
            }

            _output.Write(buffer, 0, bytes);

            GetData();
        }

        void RaiseComplete()
        {
            var handler = Completed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
