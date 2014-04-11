using System;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace logv.http
{
    public class ServerGetObserverable : IObservable<Tuple<HttpListenerRequest,IServerResponse>>
    {
        private readonly Server _server;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


        private readonly IList<IObserver<Tuple<HttpListenerRequest, IServerResponse>>> _observers =
            new List<IObserver<Tuple<HttpListenerRequest, IServerResponse>>>();

        public ServerGetObserverable(Server server)
        {
            _server = server;
            server.Get("/", (req, res) =>
            {
                _lock.EnterReadLock();
                foreach (var obs in _observers)
                {
                    obs.OnNext(Tuple.Create(req, res));
                }
                _lock.ExitReadLock();
            });
        }

        #region IObservable implementation

        public IDisposable Subscribe(IObserver<Tuple<HttpListenerRequest, IServerResponse>> observer)
        {
            _lock.EnterWriteLock();
            _observers.Add(observer);
            _lock.EnterWriteLock();

            return new GenericDisposable(() => {
                _lock.EnterWriteLock();
                _observers.Remove(observer);
                _lock.ExitWriteLock();
            });
        }

        #endregion
    }
}

