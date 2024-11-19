using KindMen.Uxios.Http;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class FileResponse : ExpectedTypeOfResponse
    {
        public string Path;

        public override void AddMetadataToRequest(Request request)
        {
            request.Headers.TryAdd(Headers.Accept, "application/octet-stream");
        }
    }
}