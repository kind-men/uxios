using KindMen.Uxios.ExpectedTypesOfResponse;
using Newtonsoft.Json;

namespace KindMen.Uxios.Interceptors
{
    public class JsonConverter
    {
        public readonly int Priority = Interceptors.DefaultSystemPriority;

        public Response OnResponseSuccess(Response response)
        {
            response.Data = response.Config.TypeOfResponseType switch
            {
                JsonResponse expectedResponse => AsJsonObject(response.Data as string, expectedResponse),
                _ => response.Data
            };

            return response;
        }

        private static object AsJsonObject(string jsonText, JsonResponse expectedResponse)
        {
            var expectedType = expectedResponse.DeserializeAs;
            var settings = expectedResponse.Settings;

            object asJsonObject;
            try
            {
                asJsonObject = JsonConvert.DeserializeObject(jsonText, expectedType, settings);
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException("Unable to parse response as JSON, received: " + jsonText, e);
            }

            return asJsonObject;
        }
    }
}