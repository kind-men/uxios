using System;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class TextResponse : ExpectedTypeOfResponse
    {
        public override void AddResponseMetadataToConfig(Config config)
        {
            config.Headers.TryAdd("Accept", "text/*");
        }
    }
}