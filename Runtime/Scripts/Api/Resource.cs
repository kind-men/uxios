using System;
using KindMen.Uxios.Errors.Http;
using RSG;

namespace KindMen.Uxios.Api
{
    // TODO: Add tests whether Value contains a value or shoudl be refreshed
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

        public Promise<bool> HasValue
        {
            get
            {
                return (Promise<bool>)this.uxios.Head(iri)
                    .Then(_ => true)
                    .Catch(error =>
                    {
                        // Resource does not exist if NotFoundError is thrown
                        if (error is NotFoundError) return false;

                        throw error;
                    });
            }
        }

        public Promise<T> Value
        {
            get
            {
                // Transparently return the cached value; no need to refresh it unless it goes stale
                if (_cachedValue != null)
                {
                    return (Promise<T>)new Promise<T>().Then(arg => _cachedValue);
                }

                return (Promise<T>)FetchResourceAsync().Then(arg => _cachedValue = arg);
            }
        }

        // Set or update the resource with new data
        public Promise<T> Update(T updatedValue)
        {
            return (Promise<T>)UpdateResourceAsync(updatedValue).Then(arg => _cachedValue = updatedValue);
        }

        public Promise RemoveAsync()
        {
            return (Promise)DeleteResourceAsync().Then(() => _cachedValue = default);
        }

        private Promise<T> FetchResourceAsync()
        {
            return (Promise<T>)this.uxios.Get<T>(iri)
                .Then(response => response.Data as T);
        }

        private Promise<T> UpdateResourceAsync(T updatedResource)
        {
            return (Promise<T>)this.uxios.Put<T, T>(iri, updatedResource)
                .Then(response => response.Data as T);
        }

        private Promise DeleteResourceAsync()
        {
            return (Promise)this.uxios.Delete(iri).Then(response => { _cachedValue = default; });
        }        
    }
}