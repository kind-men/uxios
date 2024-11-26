using RSG;

namespace KindMen.Uxios
{
    public interface IUxiosTransport
    {
        public string[] SupportedSchemes => new string[]{};
        
        Promise<IResponse> PerformRequest<TData>(Config config) where TData : class;
    }
}