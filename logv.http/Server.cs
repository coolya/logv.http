
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
using System.Threading.Tasks;

namespace logv.http
{
    /// <summary>
    /// The HTTP Server implementation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class Server 
    {


        private readonly HttpListener _listener;
        

        private readonly Dictionary<string,
            Dictionary<HttpVerb, Action<HttpListenerRequest, IServerResponse>>> _handlerByUrlAndVerb
            = new Dictionary<string, Dictionary<HttpVerb, Action<HttpListenerRequest, IServerResponse>>>();

        /// <summary>
        /// Creates a server instance
        /// </summary>
        /// <param name="root">listing root (ip or hostname or * for all requests)</param>
        /// <param name="port">listening port></param>
        public Server(string root, int port)
        {
            _listener = new  HttpListener();
            var serverAdress = string.Format("http://{0}:{1}/", root, port);
            _listener.Prefixes.Add(serverAdress);
        }

        /// <summary>
        /// Adds another http address to server
        /// </summary>
        /// <param name="root">listing root (ip or hostname or * for all requests)</param>
        /// <param name="port">listening port></param>
        public void AddAddress(string root, int port)
        {
            var serverAdress = string.Format("http://{0}:{1}/", root, port);
            _listener.Prefixes.Add(serverAdress);
        }

        /// <summary>
        /// Adds another https address to server
        /// </summary>
        /// <param name="root">listing root (ip or hostname or * for all requests)</param>
        /// <param name="port">listening port></param>
        public void AddSecureAddress(string root, int port)
        {
            var serverAdress = string.Format("https://{0}:{1}/", root, port);
            _listener.Prefixes.Add(serverAdress);
        }

        /// <summary>
        /// Adds a handler for a url and http method
        /// </summary>
        /// <param name="url">the full url the listener is handling</param>
        /// <param name="verb">the http method to handle</param>
        /// <param name="act">callback to the handler</param>
        private void AddHandler(string url, HttpVerb verb, Action<HttpListenerRequest, IServerResponse> act)
        {

            if (!_handlerByUrlAndVerb.ContainsKey(url))
                _handlerByUrlAndVerb.Add(url,
                    new Dictionary<HttpVerb, Action<HttpListenerRequest, IServerResponse>>());

            _handlerByUrlAndVerb[url].Add(verb, act);
        }

        /// <summary>
        /// Starts the http server
        /// </summary>
        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);
        }

        /// <summary>
        /// Adds a listener for a get request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Get(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
              AddHandler(url, HttpVerb.Get, act);
        }

        /// <summary>
        /// Adds a listener for a put request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Put(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
            AddHandler(url, HttpVerb.Put, act);
        }

        /// <summary>
        /// Adds a listener for a post request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Post(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
            AddHandler(url, HttpVerb.Post, act);
        }

        /// <summary>
        /// Adds a listener for a delete request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Delete(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
            AddHandler(url, HttpVerb.Delete, act);
        }

        /// <summary>
        /// Adds a listener for a patch request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Patch(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
            AddHandler(url, HttpVerb.Patch, act);
        }

        /// <summary>
        /// Adds a listener for a head request
        /// </summary>
        /// <param name="url">the full url to handle</param>
        /// <param name="act">callback to the handler</param>
        public void Head(string url, Action<HttpListenerRequest, IServerResponse> act)
        {
            AddHandler(url, HttpVerb.Head, act);
        }

        /// <summary>
        /// handles an incomming request from the async call
        /// </summary>
        /// <param name="result"></param>
        private void IncommingRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);

            Task.Factory.StartNew(() =>
                                      {

                                          HttpListenerRequest request = context.Request;
                                          HttpListenerResponse response = context.Response;


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
                                                  case "HEAD":
                                                      verb = HttpVerb.Head;
                                                      break;
                                                  case "PATCH":
                                                      verb = HttpVerb.Patch;
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
                                                          const string message =
                                                              "Message {0} /r/nSource {1}/r/n Stacktrace {2}";

                                                          var data = string.Format(message,
                                                                                   ex.Message, ex.Source, ex.StackTrace);
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
                                      });

            _listener.BeginGetContext(new AsyncCallback(IncommingRequest), _listener);
        }

        /// <summary>
        /// Gets the handler for a request. They are distinguished by the full url.
        /// If two handler A for http://localhost and B for http://localhost/sample are registered
        /// the function will choose based on the url which callback to invoke. A request for http://localhost/sample 
        /// will be handled by the B, a request for http://localhost/fail will be handled by A but a request for http://localhost/sam
        /// will be handled by B.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<HttpVerb, Action<HttpListenerRequest, IServerResponse>> GetHandlers(HttpListenerRequest request)
        {
            var uri = request.Url.ToString();
            
            if (_handlerByUrlAndVerb.ContainsKey(uri))
                return _handlerByUrlAndVerb[uri];

            //ok no 100% match, lets find the best handler
            var keys = _handlerByUrlAndVerb.Keys;

            int lastBestMatch = -1;
            string lastMatchKey = string.Empty;

            foreach (var key in keys)
            {
                //if the match can't get better the one we have we ignore it
                if (key.Length < lastBestMatch)
                    continue;

                if (key.Length > uri.Length)
                    continue;

                if (uri.Substring(0, key.Length).Equals(key))
                {
                    if (key.Length > lastBestMatch)
                    {
                        lastBestMatch = key.Length;
                        lastMatchKey = key;
                    }
                }               
            }

            if (lastBestMatch != -1)
                return _handlerByUrlAndVerb[lastMatchKey];
            else
                return null;
        }

        /// <summary>
        /// Stops the http server
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
