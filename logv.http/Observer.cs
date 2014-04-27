using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logv.http
{
    class Observer
    {
        public static ObserverT<T> Create<T>(Action onComplete, Action<T> onNext, Action<Exception> onError)
        {
            return new ObserverT<T>(onComplete, onNext, onError);
        }
    }
}
