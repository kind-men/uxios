using RSG;
using UnityEngine;

namespace KindMen.Uxios.UI
{
    public class PromiseLoaderPanel : MonoBehaviour
    {
        /// <summary>
        /// Example of a panel that is open during a request, it doesn't matter if it errors or completes because we
        /// use the `Finally` statement, which triggers in either situation.
        ///
        /// This could also have been solved using Interceptors, but that I want to keep this sample simple and show
        /// interceptors in a dedicated sample.
        /// </summary>
        public IPromise<TData> ShowWhile<TData>(IPromise<TData> promise)
        {
            gameObject.SetActive(true);

            return promise.Finally(() => gameObject.SetActive(false));
        }
    }
}