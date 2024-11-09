namespace KindMen.Uxios.Http
{
    public class BasicAuthenticationCredentials : Credentials, ICredentialsUsingAuthorizationToken
    {
        private readonly string username;
        private readonly string password;

        public BasicAuthenticationCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string ToAuthorizationToken()
        {
            var utf8ByteArray = System.Text.Encoding
                .GetEncoding("UTF-8")
                .GetBytes($"{username}:{password}");
    
            return "Basic " + System.Convert.ToBase64String(utf8ByteArray);
        }
    }
}