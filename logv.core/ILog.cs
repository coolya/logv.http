namespace logv.core
{
    public interface ILog
    {
        void Verbose(string message);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);
    }
}