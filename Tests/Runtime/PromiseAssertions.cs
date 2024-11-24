using System;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using RSG;

namespace KindMen.Uxios.Tests
{
    public static class PromiseAssertions
    {
        public static IEnumerator AssertPromiseSucceeds<TResponse>(Promise<TResponse> promise, Action<TResponse> onSuccess)
        {
            return AssertPromise(
                promise,
                onSuccess,
                e =>
                {
                    var errorMessage = "An error should not have occurred, received: " + e.Message;
                    errorMessage += "\n" + JsonConvert.SerializeObject((e as Error)?.Response, Formatting.Indented);
                    errorMessage += "\n StackTrace: " + e.InnerException?.StackTrace;

                    Assert.Fail(errorMessage);
                });
        }

        public static IEnumerator AssertPromiseErrorsWithMessage<TResponse>(Promise<TResponse> promise, string startsWith)
        {
            return AssertPromiseErrors(
                promise,
                e => Assert.That(e.Message, Does.StartWith(startsWith))
            );
        }

        public static IEnumerator AssertPromiseErrors<TResponse>(Promise<TResponse> promise, Action<Exception> onError)
        {
            return AssertPromise(promise, UnexpectedSuccess, onError);
        }

        private static void UnexpectedSuccess<TResponse>(TResponse response)
        {
            Assert.Fail("It was not expected for this to succeed, received: " + response);
        }

        private static IEnumerator AssertPromise<TResponse>(
            Promise<TResponse> promise,
            Action<TResponse> onSuccess,
            Action<Exception> onError
        )
        {
            // Assertions should be curried outside of the promise, due to exception handling
            Action assertResponse = () => { };

            promise
                // Double dispatch the onSuccess action to inject the response, but delay execution until the end
                .Then(response => assertResponse = () => onSuccess(response))
                // Double dispatch the onError action to inject the response, but delay execution until the end
                .Catch(e => assertResponse = () => onError(e));

            // Wait for the request to complete because assertions cannot happen within the Then and Catch mechanisms
            // because promises catch exceptions, including AssertExceptions 
            yield return Uxios.WaitForRequest(promise);

            // Execute the curried response so that any assertion failure exception will properly bubble up
            assertResponse();
        }
    }
}