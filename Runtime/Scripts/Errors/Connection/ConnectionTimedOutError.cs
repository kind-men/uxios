namespace KindMen.Uxios.Errors.Connection
{
    public sealed class ConnectionTimedOutError : ConnectionError
    {
        public ConnectionTimedOutError(string message, Config config) : base(message, config)
        {
        }
    }
}