using System;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class ArrayBufferResponse : ExpectedTypeOfResponse
    {
        public override void AddMetadataToRequest(Request request)
        {
            request.Headers.TryAdd("Accept", "application/octet-stream");
        }
    }
}