using System;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public class TextResponse : ExpectedTypeOfResponse
    {
        public override void AddMetadataToRequest(Request request)
        {
            request.Headers.TryAdd("Accept", "text/*");
        }
    }
}