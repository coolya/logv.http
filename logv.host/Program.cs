
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
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using logv.core;
using logv.core.PlugInModel;
using logv.http;

namespace logv.host
{
    class Program : IApplication
    {
        private static readonly List<string> Hosts = new List<string>();
        private static readonly List<string> PluginApps = new List<string>();
        private static readonly Dictionary<string,Type> PlugIns = new Dictionary<string, Type>();
        private static readonly Dictionary<string, IPlugIn> InstancesByName = new Dictionary<string, IPlugIn>();
        private static Server _server;

        static void Main(string[] args)
        {
            var rootApp = new Program {Log = new ConsoleLog()};

            GenericCommandLineParser.SetUp("host" ,val => Hosts.Add(val));
            GenericCommandLineParser.SetUp("plugin", val => PluginApps.Add(val));
            GenericCommandLineParser.Parse(args);

            _server = new Server(Hosts.First());

            foreach (var host in Hosts.Skip(1))
            {
                _server.AddAddress(host);
            }
    

            var files = Directory.GetFiles(Assembly.GetExecutingAssembly().CodeBase);

            foreach (var file in files)
            {
                try
                {
                    var asm = Assembly.LoadFrom(file);
                    foreach (var type in asm.GetTypes())
                    {
                        if (!type.IsClass || type.IsAbstract ||
                            type.GetCustomAttributes(typeof (PlugInAttribute), false).Length == 0) continue;

                        var info =  (PlugInAttribute)type.GetCustomAttributes(typeof (PlugInAttribute), false)[0];
                        PlugIns.Add(info.Name, type);
                    }
                }
                catch
                {
                }
            }

            foreach (var app in PluginApps)
            {
                var type = PlugIns[app];
                var instance = (IPlugIn) Activator.CreateInstance(type);
                instance.Register(rootApp);
                InstancesByName.Add(app, instance);
            }

            _server.Start();
        }

        public void LoadPlugin(string name)
        {
            if (InstancesByName.ContainsKey(name)) return;

            var type = PlugIns[name];
            var instance = (IPlugIn) Activator.CreateInstance(type);
            instance.Register(this);
            InstancesByName.Add(name, instance);
        }

        public IServiceLocator ServiceLocator { get; private set; }
        public ILog Log { get; private set; }
        public void Get(string uri, Action<HttpListenerRequest, IServerResponse> getAction)
        {
            _server.Get(uri, getAction);
        }

        public void Put(string uri, Action<HttpListenerRequest, IServerResponse> putAction)
        {
            _server.Put(uri, putAction);
        }

        public void Post(string uri, Action<HttpListenerRequest, IServerResponse> postAction)
        {
            _server.Post(uri, postAction);
        }

        public void Delete(string uri, Action<HttpListenerRequest, IServerResponse> deleteAction)
        {
            _server.Delete(uri, deleteAction);
        }
    }
}
