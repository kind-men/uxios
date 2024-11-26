using System.Collections.Generic;
using System.Net;
using KindMen.Uxios.Http;
using RSG;

namespace KindMen.Uxios.Transports
{
    internal sealed class InMemoryTransport : IUxiosTransport
    {
        private static InMemoryTransport inMemoryTransport;
        private readonly List<IResponse> transactions = new();

        public List<IResponse> Transactions => transactions;

        public string[] SupportedSchemes => new[] { "memory" };

        public static InMemoryTransport Instance()
        {
            if (inMemoryTransport != null) return inMemoryTransport;

            inMemoryTransport = new InMemoryTransport();

            return inMemoryTransport;
        }

        public Promise<IResponse> PerformRequest<TData>(Config config) where TData : class
        {
            var promise = new Promise<IResponse>();

            DoRequest<TData>(config, promise);

            return promise;
        }

        private void DoRequest<TData>(Config config, Promise<IResponse> promise) where TData : class
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

        private IResponse SendRequest(Config config, Request uxiosRequest)
        {
            // TODO: instead of returning a generic response, we could add stubbing support for tests
            IResponse response = new FakedWebResponse(
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