using System;

namespace logv.core
{
    public interface IServiceLocator
    {
        T GetInstance<T>();
        T GetSingleton<T>();
        void Register<T>(Func<T> creator);
        void Register<T>(Type type);
    }
}