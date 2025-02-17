using KindMen.Uxios.Http;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class ArrayBufferResponse : ExpectedTypeOfResponse
    {
        public override void AddMetadataToRequest(Request request)
        {
            // We do not add accept headers because we rely on the default handling
            // of the browser - previously I used `application/octetstream`, but some
            // webserver configuration started rejecting requests using a 406 error;
            // and this will reduce CORS preflight requests
        }
    }
}