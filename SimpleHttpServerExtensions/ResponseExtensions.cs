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
    public static class ResponseExtensions
    {
        public static ServerResponse WriteAsJson(this ServerResponse res, object obj)
        {
            res.Write(HtmlHelper.GetJson(obj));
            return res;
        }

        public static void ServeFile(this ServerResponse res, string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("File does not exist");

            var fileInfo = new FileInfo(path);
            res.ContentLength64 = fileInfo.Length;

            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var copier = new AsyncStreamCopier(stream, res.OutputStream);

            copier.Completed += (s, e) =>
            {
                e.InputStream.Close();
                res.Close();
            };

            copier.Copy();
        }

        public static ServerResponse Do503(this ServerResponse res, Exception ex)
        {
            res.StatusCode = 503;
            res.StatusDescription = ex.Message.Length < 513 ? ex.Message.Replace("\r\n", "") : string.Empty;
            return res;
        }

        public static ServerResponse Do503(this ServerResponse res)
        {
            res.StatusCode = 503;
            return res;
        }

        public static ServerResponse Write(this ServerResponse res, string data)
        {
            var bytez = Encoding.UTF8.GetBytes(data);
            res.OutputStream.Write(bytez, 0, bytez.Length);
            return res;
        }

        public static ServerResponse GetCachedResponse(this ServerResponse res)
        {
            return new CachedResponse(res);
        }

    }
}
