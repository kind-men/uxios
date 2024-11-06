using System;
using KindMen.Uxios.Errors.Http;
using RSG;

namespace KindMen.Uxios.Api
{
    // TODO: Add cache staleness checks?
    public class Resource<T> where T : class
    {
        private readonly Uri iri;
        private T _cachedValue;
        private readonly Uxios uxios;

        public Resource(Uri iri)
        {
            this.iri = iri;
            this.uxios = Uxios.DefaultInstance;
        }

        public static Resource<T> At(Uri url)
        {
            return new Resource<T>(url);
        }

        public static Resource<T> At(string url)
        {
            return new Resource<T>(new Uri(url));
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

        public Promise<T> Value
        {
            get
            {
                // Transparently return the cached value; no need to refresh it unless it goes stale
                if (_cachedValue != null)
                {
                    return Promise<T>.Resolved(_cachedValue) as Promise<T>;
                }

                return FetchResourceAsync().Then(arg => _cachedValue = arg) as Promise<T>;
            }
        }

        // Set or update the resource with new data
        public Promise<T> Update(T updatedValue)
        {
            return UpdateResourceAsync(updatedValue).Then(arg => _cachedValue = updatedValue) as Promise<T>;
        }

        public Promise Remove()
        {
            return DeleteResourceAsync();
        }

        private Promise<T> FetchResourceAsync()
        {
            return uxios.Get<T>(iri)
                .Then(response => response.Data as T) as Promise<T>;
        }

        private Promise<T> UpdateResourceAsync(T updatedResource)
        {
            return uxios.Put<T, T>(iri, updatedResource)
                .Then(response => response.Data as T) as Promise<T>;
        }

        private Promise DeleteResourceAsync()
        {
            return uxios.Delete(iri).Then(_ => { _cachedValue = default; }) as Promise;
        }        
    }
}