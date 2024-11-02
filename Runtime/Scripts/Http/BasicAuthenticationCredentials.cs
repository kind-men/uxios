namespace KindMen.Uxios.Http
{
    public class BasicAuthenticationCredentials : Credentials
    {
        public readonly string Username;
        public readonly string Password;

        public BasicAuthenticationCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string ToAuthorizationToken()
        {
            var utf8ByteArray = System.Text.Encoding
                .GetEncoding("UTF-8")
                .GetBytes($"{Username}:{Password}");
    
            return "Basic " + System.Convert.ToBase64String(utf8ByteArray);
        }
    }
}