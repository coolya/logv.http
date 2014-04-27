using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logv.http
{
    class ObserverT<T> : IObserver<T>
    {
        private readonly Action _onComplete;
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;

        public ObserverT(Action onComplete, Action<T> onNext, Action<Exception> onError)
        {
            _onComplete = onComplete;
            _onNext = onNext;
            _onError = onError;
        }

        public void OnNext(T value)
        {
            _onNext(value);
        }

        public void OnError(Exception error)
        {
            _onError(error);
        }

        public void OnCompleted()
        {
            _onComplete();
        }

        
    }
}
