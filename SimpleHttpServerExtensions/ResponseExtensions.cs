using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleHttpServer;

namespace SimpleHttpServerExtensions
{
    public static class ResponseExtensions
    {
        public static ServerResponse WriteAsJson(this ServerResponse res, object obj)
        {
            res.Write(HtmlHelper.GetJson(obj));
            return res;
        }
    }
}
