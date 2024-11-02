namespace KindMen.Uxios.Errors
{
    public class ProtocolError : Error
    {
        public ProtocolError(Config request, Response response) : base(request, response)
        {
        }

        public ProtocolError(string message, Config request, Response response) : base(message, request, response)
        {
        }
    }
}