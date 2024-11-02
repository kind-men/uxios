namespace KindMen.Uxios.Errors
{
    public class DataProcessingError : Error
    {
        public DataProcessingError(Config request, Response response) : base(request, response)
        {
        }

        public DataProcessingError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}