using RSG;

namespace KindMen.Uxios
{
    public interface IRequestRunner
    {
        Promise<Response> PerformRequest(Config config);
    }
}