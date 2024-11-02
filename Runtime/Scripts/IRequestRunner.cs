using RSG;

namespace KindMen.Uxios
{
    public interface IRequestRunner
    {
        Promise<Response> PerformRequest<TData>(Config config) where TData : class;
    }
}