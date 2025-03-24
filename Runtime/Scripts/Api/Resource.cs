using System;
using System.Collections.Generic;
using KindMen.Uxios.Errors.Http;
using KindMen.Uxios.Http;
using RSG;

namespace KindMen.Uxios.Api
{
    // TODO: Add cache staleness checks?
    public class Resource<TResponse> where TResponse : class
    {
        private readonly Uri iri;
        private TResponse _cachedValue;
        private readonly Uxios uxios;
        private readonly Config config;

        public Resource(Uri iri)
        {
            this.iri = iri;
            this.uxios = Uxios.DefaultInstance;
            this.config = new Config()
            {
                Url = iri
            };
        }

        public static Resource<TResponse> At(Uri url)
        {
            return new Resource<TResponse>(url);
        }

        public static Resource<TResponse> At(string url)
        {
            return new Resource<TResponse>(new Uri(url));
        }

        /// <summary>
        /// When it is time, get the resource with the given set of query parameters.
        /// </summary>
        public Resource<TResponse> With(QueryParameters parameters)
        {
            // Changing these parameters will make the cache invalid
            InvalidateCache();
            config.Params = parameters;

            return this;
        }

        /// <summary>
        /// When it is time, get the resource with the given query parameter in addition to other defined ones.
        /// </summary>
        public Resource<TResponse> With(string key, string value)
        {
            // Changing these parameters will make the cache invalid
            InvalidateCache();
            config.Params.Add(key, value);

            return this;
        }

        /// <summary>
        /// When it is time, get the resource with the given set of headers.
        /// </summary>
        public Resource<TResponse> With(Headers headers)
        {
            // Changing these parameters will make the cache invalid
            InvalidateCache();
            config.Headers = headers;

            return this;
        }

        /// <summary>
        /// When it is time, get the resource with the given set of headers.
        /// </summary>
        public Resource<TResponse> With(Header header)
        {
            // Changing these parameters will make the cache invalid
            InvalidateCache();
            config.Headers.Add(header);

            return this;
        }

        /// <summary>
        /// When it is time, get or update the resource with the given data as its body.
        /// </summary>
        public Resource<TResponse> With<TRequestData>(TRequestData data)
        {
            // Changing these parameters will make the cache invalid
            InvalidateCache();
            config.Data = data;

            return this;
        }

        /// <summary>
        /// Perform this request 'as' the user with the given username and password.
        /// </summary>
        public Resource<TResponse> As(string username, string password)
        {
            return As(new BasicAuthenticationCredentials(username, password));
        }

        /// <summary>
        /// Perform this request 'as' the user with the given credentials.
        /// </summary>
        public Resource<TResponse> As(Credentials credentials)
        {
            config.Auth = credentials;

            return this;
        }

        public Promise<bool> HasValue
        {
            get
            {
                return this.uxios.Head(iri)
                    .Then(_ => true)
                    .Catch(error =>
                    {
                        // Resource does not exist if NotFoundError is thrown
                        if (error is NotFoundError) return false;

                        throw error;
                    }) as Promise<bool>;
            }
        }

        public Promise<TResponse> Value
        {
            get
            {
                // Transparently return the cached value; no need to refresh it unless it goes stale
                if (_cachedValue != null)
                {
                    return Promise<TResponse>.Resolved(_cachedValue) as Promise<TResponse>;
                }

                return FetchResourceAsync().Then(arg => _cachedValue = arg) as Promise<TResponse>;
            }
        }

        // Set or update the resource with new data
        public Promise<TResponse> Update(TResponse updatedValue)
        {
            return UpdateResourceAsync(updatedValue).Then(arg => _cachedValue = updatedValue) as Promise<TResponse>;
        }

        public Promise Remove()
        {
            return DeleteResourceAsync();
        }

        private Promise<TResponse> FetchResourceAsync()
        {
            return uxios.Get<TResponse>(iri, config)
                .Then(response => response.Data as TResponse) as Promise<TResponse>;
        }

        private Promise<TResponse> UpdateResourceAsync(TResponse updatedResource)
        {
            return uxios.Put<TResponse, TResponse>(iri, updatedResource, config)
                .Then(response => response.Data as TResponse) as Promise<TResponse>;
        }

        private Promise DeleteResourceAsync()
        {
            return uxios.Delete(iri, config).Then(_ => { _cachedValue = default; }) as Promise;
        }

        private void InvalidateCache()
        {
            _cachedValue = null;
        }
    }
}