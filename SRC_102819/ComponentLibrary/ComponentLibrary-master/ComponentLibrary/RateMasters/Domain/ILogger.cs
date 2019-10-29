namespace ComponentLibrary.RateMasters.Domain
{
    public interface ILogger
    {
        void Debug(string msg);
        void Warn(string msg);
        void Info(string msg);
        void Error(string msg);
        void Fatal(string msg);
    }
}