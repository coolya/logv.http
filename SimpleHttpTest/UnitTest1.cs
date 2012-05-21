using System;
using System.Net;
using System.Text;
using SimpleHttpServer;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleHttpTest
{
    [TestClass]
    public class UnitTest1
    {
        private SimpleHttpServer.Server _server;

        [TestInitialize]
        public void Setup()
        {
            _server = new SimpleHttpServer.Server("localhost", 13337);
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

            _server.Get("", (r, s) =>
                {
                    s.ContentLength64 = count;
                    s.Write(msg);
                    complete = true;
                    mutex.Set();
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

            _server.Put("", (r, s) =>
            {
                Assert.AreEqual(msg, r.Content());
                Assert.AreEqual(count, r.ContentLength64);

                s.ContentLength64 = count;
                s.Write(msg);
                complete = true;
                mutex.Set();
            });

            _server.Start();

            var request = WebRequest.Create("http://localhost:13337");
            request.Method = "PUT";
            request.ContentLength = count;
            request.Write(msg); 
            
            var response = request.GetResponse();

            mutex.WaitOne(500);

            Assert.IsTrue(complete);
        }
    }
}
