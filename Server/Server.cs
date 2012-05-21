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
using System.Net;

namespace SimpleHttpServer
{
    public class Server
    {
        private enum HttpVerb
        {
            Get,
            Post,
            Put,
            Delete
        }

        private readonly HttpListener _listener;
        private readonly string _serverAdress;

        private readonly Dictionary<string,
            Dictionary<HttpVerb, Action<HttpListenerRequest, ServerResponse>>> _handlerByPrefixAndVerb
            = new Dictionary<string, Dictionary<HttpVerb, Action<HttpListenerRequest, ServerResponse>>>();

        public Server(string root, int port)
        {
            _listener = new  HttpListener();
            _serverAdress = string.Format("http://{0}:{1}/", root, port);
        }

        private void AddHandler(string prefix, HttpVerb verb, Action<HttpListenerRequest, ServerResponse> act)
        {

            string fulladress;
            if (prefix.EndsWith("/") || string.IsNullOrEmpty(prefix))
                fulladress = _serverAdress + prefix;
            else 
                fulladress  = _serverAdress + prefix + "/";

            _listener.Prefixes.Add(fulladress);

            if (!_handlerByPrefixAndVerb.ContainsKey(prefix))
                _handlerByPrefixAndVerb.Add(prefix,
                    new Dictionary<HttpVerb, Action<HttpListenerRequest, ServerResponse>>());

            _handlerByPrefixAndVerb[prefix].Add(verb, act);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);
        }

        public void Get(string prefix, Action<HttpListenerRequest, ServerResponse> act)
        {
              AddHandler(prefix, HttpVerb.Get, act);
        }

        public void Put(string prefix, Action<HttpListenerRequest, ServerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Put, act);
        }

        public void Post(string prefix, Action<HttpListenerRequest, ServerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Post, act);
        }

        public void Delete(string prefix, Action<HttpListenerRequest, ServerResponse> act)
        {
            AddHandler(prefix, HttpVerb.Delete, act);
        }


        private void IncommingRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);

            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);
            
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;            
            
            string url = request.RawUrl.Substring(1, request.RawUrl.Length -1) ;

            var handlers = GetHandlers(request);

            if (handlers != null)
            {
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
                {
                    try
                    {
                        handlers[verb](request, new ServerResponse(response));
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = 503;
                        response.StatusDescription = "Server Error";

                        if (request.IsLocal)
                        {                            
                                const string message = "Message {0} /r/nSource {1}/r/n Stacktrace {2}";

                                var data = string.Format(message, 
                                    ex.Message,ex.Source, ex.StackTrace);                                
                        }

                        response.Close();
                    }

                }
                else
                {
                    response.StatusCode = 405;
                    response.StatusDescription = "No handler for this HTTP method";
                    response.Close();
                }
            }
            else
            {
                response.StatusCode = 404;
                response.StatusDescription = "Not Found";
                response.Close();
            }
        }

        private Dictionary<HttpVerb, Action<HttpListenerRequest, ServerResponse>> GetHandlers(HttpListenerRequest request)
        {
            var uri = request.Url;
            

            //we have a single generic handler
            if(_handlerByPrefixAndVerb.Count == 1 && _handlerByPrefixAndVerb.ContainsKey(""))
                return _handlerByPrefixAndVerb[""];

            string urlWithoutQuery;
            if (!string.IsNullOrEmpty(uri.Query))
                urlWithoutQuery = uri.PathAndQuery.Replace(uri.Query, "");
            else
                urlWithoutQuery = uri.PathAndQuery;

            if (urlWithoutQuery[0] == '/')
                urlWithoutQuery = urlWithoutQuery.Substring(1);

            if (_handlerByPrefixAndVerb.ContainsKey(urlWithoutQuery))
                return _handlerByPrefixAndVerb[urlWithoutQuery];

            //ok no 100% match, lets find the best handler
            var keys = _handlerByPrefixAndVerb.Keys;

            int lastBestMatch = -1;
            string lastMatchKey = string.Empty;

            foreach (var key in keys)
            {
                //if the match can't get better the one we have we ignore it
                if (key.Length < lastBestMatch)
                    continue;

                int k;
                for (k = 1; k <= key.Length; k++)
                {
                    if(!urlWithoutQuery.StartsWith(key.Substring(0, k)))
                    {
                        if (lastBestMatch < k)
                        {
                            lastBestMatch = k;
                            lastMatchKey = key;
                        }
                        break;
                    }
                }

                if (k - 1 == key.Length && lastBestMatch < k)
                {
                    lastBestMatch = k;
                    lastMatchKey = key;
                }
            }

            if (lastBestMatch != -1)
                return _handlerByPrefixAndVerb[lastMatchKey];
            else
                return null;
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
