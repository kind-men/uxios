﻿namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class SpriteResponse : ExpectedTypeOfResponse
    {
        public bool Readable = false;

        public override void AddMetadataToRequest(Request request)
        {
            request.Headers.TryAdd("Accept", "image/*");
        }
    }
}