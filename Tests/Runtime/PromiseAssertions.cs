using System;
using System.Collections;
using NUnit.Framework;
using RSG;

namespace KindMen.Uxios.Tests
{
    public static class PromiseAssertions
    {
        public static IEnumerator AssertPromiseSucceeds(Promise<Response> promise, Action<Response> onSuccess)
        {
            return AssertPromise(
                promise,
                onSuccess,
                e => Assert.Fail("An error should not have occurred, received: " + e.Message)
            );
        }

        public static IEnumerator AssertPromiseErrorsWithMessage(Promise<Response> promise, string startsWith)
        {
            return AssertPromiseErrors(
                promise,
                e => Assert.That(e.Message, Does.StartWith(startsWith))
            );
        }

        public static IEnumerator AssertPromiseErrors(Promise<Response> promise, Action<Exception> onError)
        {
            return AssertPromise(promise, UnexpectedSuccess, onError);
        }

        private static void UnexpectedSuccess(Response response)
        {
            Assert.Fail("It was not expected for this to succeed, received: " + response?.Data);
        }

        private static IEnumerator AssertPromise(
            Promise<Response> promise,
            Action<Response> onSuccess,
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