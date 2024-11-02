namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class TextureResponse : ExpectedTypeOfResponse
    {
        public bool Readable = false;

        public override void AddResponseMetadataToConfig(Config config)
        {
            config.Headers.TryAdd("Accept", "image/*");
        }
    }
}