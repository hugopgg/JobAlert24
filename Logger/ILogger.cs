namespace JobAlert.Logger
{

    public interface ILogger
    {
        void Info(string message);

        void Warn(string message);


        void Debug(string message);


        void Error(string message, Exception ex);


    }




}