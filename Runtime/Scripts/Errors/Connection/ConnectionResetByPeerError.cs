namespace KindMen.Uxios.Errors.Connection
{
    public sealed class ConnectionResetByPeerError : ConnectionError
    {
        public ConnectionResetByPeerError(string message, Config config) : base(message, config)
        {
        }
    }
}