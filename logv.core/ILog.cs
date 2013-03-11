namespace logv.core
{
    public interface ILog
    {
        void Verbose(string message);
        void Verbose(string message, params object[] parameters);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Warning(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Fatal(string message, params object[] parameters);
    }
}