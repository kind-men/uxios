namespace KindMen.Uxios
{
    public interface IEnvironment
    {
        
    }

    public class Environment<TPayload> : IEnvironment where TPayload : class
    {
        public TPayload Payload { get; set; }
    }
}