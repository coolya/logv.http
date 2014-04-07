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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace logv.http.sample
{
    class Program
    {
        static void Main(string[] args)
        {
			var server = new Server("localhost", 13337);

			server.Get("http://localhost:13337/", (req, res) => res.Write("Hello World!").Close());

            //to have json output uncomment the next line and comment out the previous one.
            //server.Get("http://localhost:13337" ,(req, res) => res.WriteAsJson(new {Hello = "world"}));

            server.Start();
			Console.WriteLine("Server running");
			while (Console.ReadLine() == null)
				Thread.Sleep(100);

            server.Stop();
        }
    }
}
