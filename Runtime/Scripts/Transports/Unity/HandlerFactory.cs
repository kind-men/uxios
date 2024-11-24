using KindMen.Uxios.ExpectedTypesOfResponse;
using UnityEngine.Networking;

namespace KindMen.Uxios.Transports.Unity
{
    public static class HandlerFactory
    {
        public static DownloadHandler DownloadHandler(ExpectedTypeOfResponse expectedTypeOfResponse)
        {
            return expectedTypeOfResponse switch
            {
                TextureResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                FileResponse responseType => new DownloadHandlerFile(responseType.Path),
                SpriteResponse responseType => new DownloadHandlerTexture(responseType.Readable),
                _ => new DownloadHandlerBuffer()
            };
        }

        public static UploadHandler UploadHandler(Request request)
        {
            return new UploadHandlerRaw(request.Data ?? new byte[] { });
        }
    }
}