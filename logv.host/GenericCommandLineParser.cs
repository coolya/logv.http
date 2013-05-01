using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logv.host
{
    static class GenericCommandLineParser
    {
        public static void Parse(IEnumerable<string> parameters)
        {
            var enumerator = parameters.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                if (current.StartsWith("-"))
                {
                    
                }
            }
        }
        public static void SetUp(string param, Action setter)
        {
        }

        public static void SetUp(string param, bool multiple, Action setter)
        {
            
        }

        public static void SetUp(Action setter)
        {
            
        }
    }
}
