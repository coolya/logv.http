using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logv.http
{
    class Disposable : IDisposable
    {
        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
