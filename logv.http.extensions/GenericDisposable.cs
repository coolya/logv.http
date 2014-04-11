using System;

namespace logv.http
{
    public class GenericDisposable : IDisposable
    {
        private readonly Action _act;
        public GenericDisposable(Action toDispose)
        {
            _act = toDispose;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            _act();
        }

        #endregion
    }
}

