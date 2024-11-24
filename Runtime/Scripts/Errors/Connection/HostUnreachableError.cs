namespace KindMen.Uxios.Errors.Connection
{
    public sealed class HostUnreachableError : ConnectionError
    {
        public HostUnreachableError(string message, Config config) : base(message, config)
        {
        }
    }
}