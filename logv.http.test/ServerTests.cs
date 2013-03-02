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
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using logv.http;

namespace SimpleHttpTest
{
    [TestClass]
    public class ServerTests
    {
        private Server _server;

        [TestInitialize]
        public void Setup()
        {
            _server = new Server("localhost", 13337);
        }


        [TestCleanup]
        public void CleanUp()
        {            
            _server.Stop();
            _server = null;
        }


        [TestMethod]
        [Owner("Kolja Dummann")]
        public void GetTest()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            const string msg = "Test complete";
            int count = Encoding.UTF8.GetByteCount(msg);

            _server.Get("http://localhost:13337", (r, s) =>
                {
                    s.ContentLength64 = count;
                    s.Write(msg);
                    complete = true;
                    mutex.Set();
                    s.Close();
                });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337");
            var response = request.GetResponse();

            mutex.WaitOne(500);

            if (!complete)
                Assert.Fail();
            
            var stream = response.GetResponseStream();
            byte[] buffer = new byte[response.ContentLength];
            stream.Read(buffer, 0, (int)response.ContentLength);

            Assert.AreEqual(count, response.ContentLength);
            Assert.AreEqual("Test complete", Encoding.UTF8.GetString(buffer).Trim());
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void PutTest()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            const string msg = "Test complete";
            int count = Encoding.UTF8.GetByteCount(msg);

            _server.Put("http://localhost:13337", (r, s) =>
            {
                Assert.AreEqual(msg, r.Content());
                Assert.AreEqual(count, r.ContentLength64);

                s.ContentLength64 = count;
                s.Write(msg);
                complete = true;
                mutex.Set();
                s.Close();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337");
            request.Method = "PUT";
            request.ContentLength = count;
            request.Write(msg); 
            
            var response = request.GetResponse();

            mutex.WaitOne(500);

            Assert.IsTrue(complete);

            var stream = response.GetResponseStream();

            Assert.AreEqual(count, response.ContentLength);

            byte[] bytez = new byte[response.ContentLength];
            stream.Read(bytez, 0, (int)response.ContentLength);

            Assert.AreEqual(msg, Encoding.UTF8.GetString(bytez));
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void DeleteTest()
        {
            
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            const string msg = "Test complete";
            int count = Encoding.UTF8.GetByteCount(msg);

            _server.Delete("http://localhost:13337", (r, s) =>
            {
                Assert.AreEqual(msg, r.Content());
                Assert.AreEqual(count, r.ContentLength64);

                s.ContentLength64 = count;
                s.Write(msg);
                complete = true;
                mutex.Set();
                s.Close();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337");
            request.Method = "DELETE";
            request.ContentLength = count;
            request.Write(msg);

            var response = request.GetResponse();

            mutex.WaitOne(500);

            Assert.IsTrue(complete);

            var stream = response.GetResponseStream();

            Assert.AreEqual(count, response.ContentLength);

            byte[] bytez = new byte[response.ContentLength];
            stream.Read(bytez, 0, (int)response.ContentLength);

            Assert.AreEqual(msg, Encoding.UTF8.GetString(bytez));
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void PostTest()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            const string msg = "Test complete";
            int count = Encoding.UTF8.GetByteCount(msg);

            _server.Post("http://localhost:13337", (r, s) =>
            {
                Assert.AreEqual(msg, r.Content());
                Assert.AreEqual(count, r.ContentLength64);

                s.ContentLength64 = count;
                s.Write(msg);
                complete = true;
                mutex.Set();
                s.Close();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337");
            request.Method = "POST";
            request.ContentLength = count;
            request.Write(msg);

            var response = request.GetResponse();

            mutex.WaitOne(500);

            Assert.IsTrue(complete);

            var stream = response.GetResponseStream();

            Assert.AreEqual(count, response.ContentLength);

            byte[] bytez = new byte[response.ContentLength];
            stream.Read(bytez, 0, (int)response.ContentLength);

            Assert.AreEqual(msg, Encoding.UTF8.GetString(bytez));
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void MultipleHandlersTest()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            _server.Get("http://localhost:13337/test1", (rq, rs) =>
                {
                    complete = true;
                    mutex.Set();
                    rs.Close();
                });

            _server.Get("http://localhost:13337/test2/", (rq, rs) => Assert.Fail());

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/test1");
            var response = request.GetResponse();

            mutex.WaitOne(500);

            if (!complete)
                Assert.Fail();
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void MultipleHandlersNoExactMatchTest()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;
            _server.Get("http://localhost:13337/test1", (rq, rs) =>
            {
                complete = true;
                mutex.Set();
                rs.Close();
            });

            _server.Get("http://localhost:13337/test2/", (rq, rs) => Assert.Fail());

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/test1/test");
            var response = request.GetResponse();

            mutex.WaitOne(500);

            if (!complete)
                Assert.Fail();
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void MultipleHandlersNoExactMatch2Test()
        {
            var mutex = new AutoResetEvent(false);
            bool complete = false;

            _server.Get("http://localhost:13337/test/", (rq, rs) => Assert.Fail());

            _server.Get("http://localhost:13337/test1", (rq, rs) =>
            {
                complete = true;
                mutex.Set();
                rs.Close();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/test1/test");
            var response = request.GetResponse();

            mutex.WaitOne(500);

            if (!complete)
                Assert.Fail();
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        [ExpectedException(typeof(WebException))]
        public void ExceptionTest()
        {
            _server.Get("http://localhost:13337/test1", (rq, rs) =>
            {
                throw new Exception();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/test1");
            var response = request.GetResponse();
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        [ExpectedException(typeof(WebException))]
        public void Exception2Test()
        {
            _server.Delete("http://localhost:13337/test1", (rq, rs) =>
            {
                Assert.Fail();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/test1");
            var response = request.GetResponse();
         }

        [TestMethod]
        [Owner("Kolja Dummann")]
        [ExpectedException(typeof(WebException))]
        public void Exception3Test()
        {
            _server.Delete("http://localhost:13337/test1", (rq, rs) =>
            {
                Assert.Fail();
            });

            _server.Delete("http://localhost:13337/meh", (rq, rs) =>
            {
                Assert.Fail();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337/2");
            var response = request.GetResponse();
        }

        [TestMethod]
        [Owner("Kolja Dummann")]
        public void RootMatchTest()
        {
            _server.Get("http://localhost:13337/", (req, res) => res.Close());
            _server.Start();
            var rq = WebRequest.Create("http://localhost:13337/");
            rq.GetResponse();

            rq = WebRequest.Create("http://localhost:13337/index.html");
            rq.GetResponse();
        }

    }
}
