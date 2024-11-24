namespace KindMen.Uxios.Errors.Connection
{
    public sealed class CertificateValidationError : ConnectionError
    {
        public CertificateValidationError(string message, Config config) : base(message, config)
        {
        }
    }
}