using Newtonsoft.Json;
using UnityEngine;

namespace KindMen.Uxios.Interceptors.NetworkInspector
{
    public class ConsoleLogger : Logger
    {
        public override Config OnRequestSuccess(Config config)
        {
            var data = JsonConvert.SerializeObject(config, Formatting.Indented);

            Debug.Log($"[UXIOS] Sending request to {config.Url}: \n\n{data}");

            return config;
        }

        public override Error OnRequestError(Error error)
        {
            var data = JsonConvert.SerializeObject(error, Formatting.Indented);

            Debug.LogError($"[UXIOS] Error occurred during request to {error.Config.Url}: \n\n{data}");

            return error;
        }

        public override IResponse OnResponseSuccess(IResponse response)
        {
            var data = JsonConvert.SerializeObject(response, Formatting.Indented);

            Debug.Log($"[UXIOS] Received response from {response.Request.Url}: \n\n{data}");
            
            return response;
        }

        public override Error OnResponseError(Error error)
        {
            var data = JsonConvert.SerializeObject(error, Formatting.Indented);
            
            Debug.LogError($"[UXIOS] Error occurred at {error.Response.Request.Url}: \n\n{data}");

            return error;
        }
    }
}