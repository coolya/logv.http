using System;
using System.Net;
using logv.http;

namespace logv.core
{
    public interface IApplication
    {
        IServiceLocator ServiceLocator { get; }
        ILog Log { get; }
        void Get(string uri, Action<HttpListenerRequest, IServerResponse> getAction);
        void Put(string uri, Action<HttpListenerRequest, IServerResponse> putAction);
        void Post(string uri, Action<HttpListenerRequest, IServerResponse> postAction);
        void Delete(string uri, Action<HttpListenerRequest, IServerResponse> deleteAction);
    }
}