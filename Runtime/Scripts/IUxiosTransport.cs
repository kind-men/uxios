using RSG;

namespace KindMen.Uxios
{
    public interface IUxiosTransport
    {
        Promise<Response> PerformRequest<TData>(Config config) where TData : class;
    }
}