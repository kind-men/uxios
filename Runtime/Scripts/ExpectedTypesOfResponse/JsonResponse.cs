using System;
using Newtonsoft.Json;

namespace KindMen.Uxios.ExpectedTypesOfResponse
{
    public sealed class JsonResponse : ExpectedTypeOfResponse
    {
        public Type DeserializeAs = null;
        public JsonSerializerSettings Settings = new();
        
        /// <summary>
        /// Some (RESTful) APIs negotiate what JSON object to return by inspecting the Accept header sent by the
        /// request. With this property you can describe the type of object to return; or just ignore this to use the
        /// default "application/json".
        /// </summary>
        public string OfResourceType = "application/json";
        
        public override void AddMetadataToRequest(Request request)
        {
            request.Headers.TryAdd("Accept", OfResourceType);
        }
    }
}