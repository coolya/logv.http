using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleHttpServer
{
    public class HtmlHelper
    {
        public static string PackIntoHtml(string body)
        {
            return string.Format("<HTML><BODY>{0}</BODY></HTML>", body);
        }
    }
}
