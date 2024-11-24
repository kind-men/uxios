namespace KindMen.Uxios.Errors.Connection
{
    public sealed class DnsResolutionError : ConnectionError
    {
        public DnsResolutionError(string message, Config config) : base(message, config)
        {
        }
    }
}