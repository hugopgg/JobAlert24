namespace JobAlert.Logger
{
    public enum LogLevel
    {
        Info,
        Warn,
        Debug,
        Error
    }
    public class FileConsoleLogger : ILogger
    {
        private readonly string _logFilePath = "LogFiles/log.txt";

        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warn(string message) => Log(LogLevel.Warn, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Error(string message, Exception? ex = null)
        {
            string fullMessage = ex != null ? $"{message}\nException: {ex.Message}\nStack Trace: {ex.StackTrace}" : message;
            Log(LogLevel.Error, fullMessage);

        }

        private void Log(LogLevel level, string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}