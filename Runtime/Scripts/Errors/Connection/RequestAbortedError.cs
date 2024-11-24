namespace KindMen.Uxios.Errors.Connection
{
    public sealed class RequestAbortedError : ConnectionError
    {
        public RequestAbortedError(string message, Config config) : base(message, config)
        {
        }
    }
}