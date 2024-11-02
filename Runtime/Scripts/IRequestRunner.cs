using RSG;

namespace KindMen.Uxios
{
    public interface IRequestRunner
    {
        public void Preflight<TData>(Config config) where TData : class;

        Promise<Response> PerformRequest(Config config);
    }
}