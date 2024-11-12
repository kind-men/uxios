using UnityEngine;

namespace KindMen.Uxios.Interceptors.NetworkInspector
{
    public class ConsoleLogger : Logger
    {
        public override Config OnRequestSuccess(Config request)
        {
            Debug.Log("Received request using configured URL " + request.Url);

            return request;
        }

        public override Error OnRequestError(Error error)
        {
            Debug.LogError("Received error while executing request to " + error.Request.Url + ": " + error.Message);

            return error;
        }

        public override Response OnResponseSuccess(Response response)
        {
            Debug.Log("Received response from requested URL " + response.Request.Url);
            
            return response;
        }

        public override Error OnResponseError(Error error)
        {
            Debug.LogError("Received error while receiving response from " + error.Request.Url + ": " + error.Message);

            return error;
        }
    }
}