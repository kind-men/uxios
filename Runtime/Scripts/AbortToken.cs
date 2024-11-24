using System.Threading;

namespace KindMen.Uxios
{
    /// <summary>
    /// Represents a token that tracks the lifecycle and cancellation state of a promise.
    /// </summary>
    /// <remarks>
    /// This class is designed to work with promises that may not have an explicit lifecycle
    /// management or signaling mechanism, such as Unity Coroutines. It uses a 
    /// <see cref="CancellationTokenSource"/> to enable cancellation but is specifically 
    /// intended for promise-based workflows.
    /// </remarks>
    public class AbortToken
    {
        private CancellationTokenSource cancellationTokenSource;
        private int? failedHealthChecks;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbortToken"/> class.
        /// </summary>
        /// <param name="cancellationTokenSource">
        /// The <see cref="CancellationTokenSource"/> used to manage cancellation for this token.
        /// </param>
        public AbortToken(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        /// Cancels the promise associated with this token by triggering the underlying 
        /// <see cref="CancellationTokenSource"/>.
        /// </summary>
        public void Abort()
        {
            cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Signals that the promise is still active by resetting the internal health check counter.
        /// </summary>
        /// <remarks>
        /// This method should be used in contexts where the lifecycle of the promise is managed by 
        /// an external process that lacks a signaling mechanism for forced termination. 
        /// For example, Unity Coroutines can be stopped at any moment, but they do not notify 
        /// associated promises when this occurs.
        /// </remarks>
        public void KeepAlive()
        {
            failedHealthChecks = 0;
        }

        /// <summary>
        /// Checks whether the promise associated with this token is considered alive.
        /// </summary>
        /// <remarks>
        /// This method is designed for scenarios where a promise may be forcefully stopped by an 
        /// external process without explicit signaling.
        ///  
        /// If the health check system is not active (i.e., <see cref="KeepAlive"/> has not been called),
        /// this method will always return <c>true</c>.
        /// 
        /// If the health check system is active, this method toggles the internal state and returns 
        /// <c>true</c> for the first check after <see cref="KeepAlive"/> is called. Subsequent checks 
        /// will return <c>false</c> unless <see cref="KeepAlive"/> is called again.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the promise is considered alive; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAlive()
        {
            // it is not being monitored, which means it is always alive
            if (failedHealthChecks.HasValue == false) return true;

            // Everytime you check whether it is alive, it will flip whether it is alive. The KeepAlive method
            // should be called again before this method is called to keep the token alive
            if (failedHealthChecks == 0)
            {
                failedHealthChecks++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disposes of the resources used by the underlying <see cref="CancellationTokenSource"/>.
        /// </summary>
        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}