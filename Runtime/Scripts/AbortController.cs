using System.Collections.Generic;
using System.Threading;
using RSG;
using UnityEngine;

namespace KindMen.Uxios
{
    
    /// <summary>
    /// Manages the lifecycle and cancellation of promises by tracking associated <see cref="AbortToken"/> objects.
    /// </summary>
    /// <remarks>
    /// This class provides a centralized mechanism for monitoring and aborting promises, particularly
    /// useful in Unity contexts where external processes, such as Coroutines, may terminate unexpectedly
    /// without signaling associated promises. It uses Unity's <see cref="MonoBehaviour"/> to handle
    /// lifecycle updates through <see cref="LateUpdate"/>.
    /// </remarks>
    public class AbortController : MonoBehaviour
    {
        private static AbortController abortController;
        private readonly Dictionary<IPromiseInfo, AbortToken> abortTokens = new ();

        /// <summary>
        /// Gets the singleton instance of the <see cref="AbortController"/>.
        /// </summary>
        /// <returns>
        /// The singleton instance of the <see cref="AbortController"/>.
        /// </returns>
        /// <remarks>
        /// This instance is created as a hidden, non-destroyable GameObject to ensure it persists
        /// across scene transitions.
        /// </remarks>
        public static AbortController Instance()
        {
            if (abortController != null) return abortController;

            var gameObject = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            DontDestroyOnLoad(gameObject);

            abortController = gameObject.AddComponent<AbortController>();

            return abortController;
        }

        /// <summary>
        /// Registers a non-generic promise with the controller.
        /// </summary>
        /// <param name="promise">The promise to register.</param>
        /// <param name="cancellationSource">The <see cref="CancellationTokenSource"/> associated with the promise.</param>
        public void Register(Promise promise, CancellationTokenSource cancellationSource)
        {
            this.abortTokens[promise] = new AbortToken(cancellationSource);
        }

        /// <summary>
        /// Registers a generic promise with the controller.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="promise">The promise to register.</param>
        /// <param name="cancellationSource">The <see cref="CancellationTokenSource"/> associated with the promise.</param>
        public void Register<T>(Promise<T> promise, CancellationTokenSource cancellationSource)
        {
            this.abortTokens[promise] = new AbortToken(cancellationSource);
        }

        /// <summary>
        /// Unregisters a non-generic promise from the controller and disposes of its associated token.
        /// </summary>
        /// <param name="promise">The promise to unregister.</param>
        public void Unregister(Promise promise)
        {
            if (abortTokens.ContainsKey(promise) == false) return;

            abortTokens[promise].Dispose();
            abortTokens.Remove(promise);
        }

        /// <summary>
        /// Unregisters a generic promise from the controller and disposes of its associated token.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="promise">The promise to unregister.</param>
        public void Unregister<T>(Promise<T> promise)
        {
            if (abortTokens.ContainsKey(promise) == false) return;

            abortTokens[promise].Dispose();
            abortTokens.Remove(promise);
        }

        /// <summary>
        /// Aborts the specified promise by canceling its associated token.
        /// </summary>
        /// <param name="promise">The promise to abort.</param>
        public void Abort(IPromiseInfo promise)
        {
            if (abortTokens.ContainsKey(promise) == false) return;

            abortTokens[promise].Abort();
        }

        /// <summary>
        /// Resets the health check for the specified promise, signaling that it is still active.
        /// </summary>
        /// <param name="promise">The promise to keep alive.</param>
        public void KeepAlive(IPromiseInfo promise)
        {
            if (abortTokens.ContainsKey(promise) == false) return;

            abortTokens[promise].KeepAlive();
        }

        /// <summary>
        /// Checks the health of all tracked promises and aborts those considered no longer alive.
        /// </summary>
        /// <remarks>
        /// This method is called automatically during <see cref="LateUpdate"/> to monitor promise health.
        /// </remarks>
        private void Heartbeat()
        {
            foreach (var (promise, abortToken) in abortTokens)
            {
                if (abortToken.IsAlive()) continue;

                // the process wrapping the promise is no longer alive, we need to abort the promise
                Abort(promise);
            }
        }

        /// <summary>
        /// Performs a heartbeat check at the end of each frame to monitor promise health.
        /// </summary>
        private void LateUpdate()
        {
            Heartbeat();
        }
    }
}