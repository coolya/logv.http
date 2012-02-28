using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SimpleHttpServer
{
    public class Shs
    {
        private enum HttpVerb
        {
            Get,
            Post,
            Put,
            Delete
        }

        private HttpListener _listener;
        private string _serverAdress;

        private Dictionary<string,
            Dictionary<HttpVerb, Action<HttpListenerRequest, HttpListenerResponse>>> _handlerByPrefixAndVerb 
            = new Dictionary<string,Dictionary<HttpVerb,Action<HttpListenerRequest,HttpListenerResponse>>>();

        public Shs(string root, int port)
        {
            _listener = new  HttpListener();
            _serverAdress = string.Format("http://{0}:{1}/", root, port);
        }

        private void AddHandler(string prefix, HttpVerb verb, Action<HttpListenerRequest, HttpListenerResponse> act)
        {

            string fulladress = prefix.EndsWith("/") 
                ? _serverAdress + prefix : _serverAdress + prefix + "/";

            _listener.Prefixes.Add(fulladress);

            if (!_handlerByPrefixAndVerb.ContainsKey(prefix))
                _handlerByPrefixAndVerb.Add(prefix, 
                    new Dictionary<HttpVerb, Action<HttpListenerRequest, HttpListenerResponse>>());

            _handlerByPrefixAndVerb[prefix].Add(verb, act);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);
        }

        public void Get(string prefix, Action<HttpListenerRequest, HttpListenerResponse> act)
        {
              AddHandler(prefix, HttpVerb.Get, act);
        }



        public void Put(string prefix, Action<HttpListenerRequest, HttpListenerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Put, act);
        }

        public void Post(string prefix, Action<HttpListenerRequest, HttpListenerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Post, act);
        }

        public void Delete(string prefix, Action<HttpListenerRequest, HttpListenerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Delete, act);
        }


        private void IncommingRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);

            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string url = request.RawUrl.Substring(1, request.RawUrl.Length -1) ;

            if (_handlerByPrefixAndVerb.ContainsKey(url))
            {
                var handlers = _handlerByPrefixAndVerb[url];

                HttpVerb verb = HttpVerb.Get;

                switch (request.HttpMethod)
                {
                    case "GET":
                        verb = HttpVerb.Get;
                        break;
                    case "POST":
                        verb = HttpVerb.Post;
                        break;
                    case "PUT":
                        verb = HttpVerb.Put;
                        break;
                    case "DELETE":
                        verb = HttpVerb.Delete;
                        break;
                }

                if (handlers.ContainsKey(verb))
                    handlers[verb](request, response);
                else
                {
                    response.StatusCode = 405;
                    response.StatusDescription = "No handler for this HTTP method";

                }
            }
            else
            {
                response.StatusCode = 404;
                response.StatusDescription = "Not Found";
            }
            response.Close();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
