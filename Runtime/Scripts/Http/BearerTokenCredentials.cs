namespace KindMen.Uxios.Http
{
    public class BearerTokenCredentials : Credentials, ICredentialsUsingAuthorizationToken
    {
        private readonly string token;

        public BearerTokenCredentials(string token)
        {
            this.token = token;
        }

        public string ToAuthorizationToken()
        {
            return "Bearer " + token;
        }
    }
}