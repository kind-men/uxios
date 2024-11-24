using KindMen.Uxios.Errors.Connection;

namespace KindMen.Uxios.Errors
{
    public class ConnectionError : Error
    {
        public ConnectionError(string message, Config config) : base(message, config, null)
        {
        }

        public static ConnectionError FromString(string errorText, Config config)
        {
            if (errorText.Contains("Request aborted"))
            {
                return new RequestAbortedError(errorText, config);
            }
            if (errorText.Contains("Cannot resolve destination host"))
            {
                return new DnsResolutionError(errorText, config);
            }
            if (errorText.Contains("Failed to connect to"))
            {
                return new HostUnreachableError(errorText, config);
            }
            if (errorText.Contains("Certificate validation failed"))
            {
                return new CertificateValidationError(errorText, config);
            }
            if (errorText.Contains("Connection timed out"))
            {
                return new ConnectionTimedOutError(errorText, config);
            }
            if (errorText.Contains("Connection reset by peer"))
            {
                return new ConnectionResetByPeerError(errorText, config);
            }

            return new(errorText, config);
        }
    }
}