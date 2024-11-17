using System.Collections.Generic;
using System.Net;
using KindMen.Uxios.Http;
using RSG;

namespace KindMen.Uxios.Transports
{
    internal sealed class InMemoryTransport : IUxiosTransport
    {
        private static InMemoryTransport inMemoryTransport;
        private readonly List<Response> transactions = new();

        public List<Response> Transactions => transactions;

        public string[] SupportedSchemes => new[] { "memory" };

        public static InMemoryTransport Instance()
        {
            if (inMemoryTransport != null) return inMemoryTransport;

            inMemoryTransport = new InMemoryTransport();

            return inMemoryTransport;
        }

        public Promise<Response> PerformRequest<TData>(Config config) where TData : class
        {
            var promise = new Promise<Response>();

            DoRequest<TData>(config, promise);

            return promise;
        }

        private void DoRequest<TData>(Config config, Promise<Response> promise) where TData : class
        {
            var uxiosRequest = TransportActions.CreateRequest<TData>(ref config);
            
            var response = SendRequest(config, uxiosRequest);

            Transactions.Add(response);

            TransportActions.HandleResponse(
                promise, 
                config, 
                uxiosRequest, 
                (_, _) => response
            );
        }

        private Response SendRequest(Config config, Request uxiosRequest)
        {
            // TODO: instead of returning a generic response, we could add stubbing support for tests
            Response response = new FakedWebResponse(
                config,
                uxiosRequest, 
                HttpStatusCode.OK, 
                new Headers(), 
                null
            );
            return response;
        }
    }
}