using System;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class ArrayBufferResponse : ExpectedTypeOfResponse
    {
        public override void AddResponseMetadataToConfig(Config config)
        {
            config.Headers.TryAdd("Accept", "application/octet-stream");
        }
    }
}