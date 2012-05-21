﻿/*
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
using System.Net;
using System.Text;

namespace SimpleHttpServer
{
    public static class WebRequestExtensions
    {
        public static WebRequest Write(this WebRequest req, string data)
        {
            var bytez = Encoding.UTF8.GetBytes(data);
            req.GetRequestStream().Write(bytez, 0, bytez.Length);            
            return req;
        }
    }
}