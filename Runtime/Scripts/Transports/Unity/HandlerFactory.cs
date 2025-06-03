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
            var data = request.Data ?? new byte[] { };

            // Unity will change the request headers when UploadHandlerRaw is provided - to properly handle requests we
            // do not create an uploadHandler is there is no data.
            return data.Length > 0 ? new UploadHandlerRaw(data) : null;
        }
    }
}