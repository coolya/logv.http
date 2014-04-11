using System;
using System.Net;

namespace logv.http
{
    public static class RxExtensions
    {
        public static IObservable<Tuple<HttpListenerRequest,IServerResponse>> Get(this Server server, string url)
        {
            server.Get
        }
    }
}

