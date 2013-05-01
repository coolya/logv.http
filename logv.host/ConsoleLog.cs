using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using logv.core;

namespace logv.host
{
    class ConsoleLog : ILog
    {
        public void Verbose(string message)
        {
            Console.WriteLine("[VERBOSE] -- {0}", message);
        }

        public void Verbose(string message, params object[] parameters)
        {
            Verbose(string.Format(message, parameters));
        }


        public void Debug(string message)
        {
            Console.WriteLine("[DEBUG] -- {0}", message);
        }

        public void Debug(string message, params object[] parameters)
        {
            Debug(string.Format(message, parameters));
        }

        public void Info(string message)
        {
            Console.WriteLine("[INFO] -- {0}", message);
        }

        public void Info(string message, params object[] parameters)
        {
            Info(string.Format(message, parameters));
        }

        public void Warning(string message)
        {
            Console.WriteLine("[WARNING] -- {0}", message);
        }

        public void Warning(string message, params object[] parameters)
        {
            Warning(string.Format(message, parameters));
        }

        public void Error(string message)
        {
            Console.WriteLine("[ERROR] -- {0}", message);
        }

        public void Error(string message, params object[] parameters)
        {
            Error(string.Format(message, parameters));
        }

        public void Fatal(string message)
        {
            Console.WriteLine("[FATAL] -- {0}", message);
        }

        public void Fatal(string message, params object[] parameters)
        {
            Fatal(string.Format(message,parameters));
        }
    }
}
