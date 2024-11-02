using System;
using System.Collections;
using System.Net;
using KindMen.Uxios.Interceptors;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Uri = System.Uri;

namespace KindMen.Uxios.Tests
{
    public class Interceptors
    {
        private AssertionException requestAssertionException;
        private AssertionException responseAssertionException;
        private AssertionException errorAssertionException;
        private Uxios uxios;
        private Uri url;
        private Config config;

        [SetUp]
        public void SetUp()
        {
            responseAssertionException = null;
            requestAssertionException = null;
            errorAssertionException = null;

            uxios = new Uxios();
            Uxios.Interceptors.request.Clear();
            Uxios.Interceptors.response.Clear();

            // All interceptor tests basically use the same settings
            url = new Uri("https://kind-men.com");
            config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };
        }

        [UnityTest]
        public IEnumerator InterceptSuccessfulRequests()
        {
            bool interceptionIsCalled = false;
            Uxios.Interceptors.request.Add(new(AssertRequestInterception(configArg =>
            {
                Assert.That(configArg, Is.TypeOf<Config>());
                interceptionIsCalled = true;
            })));

            var promise = uxios.Get(url, config);
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    if (requestAssertionException is not null) throw requestAssertionException;

                    Assert.That(interceptionIsCalled, Is.True);
                }
            );
        }

        [UnityTest]
        public IEnumerator InterceptSuccessfulResponses()
        {
            bool interceptionIsCalled = false;
            Uxios.Interceptors.response.Add(new(AssertResponseInterception(responseArg =>
            {
                interceptionIsCalled = true;
                Assert.That(responseArg.Config, Is.TypeOf<Config>());
                // Double check whether the request was actually successfully completed
                Assert.That(responseArg.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseArg.Data, Is.TypeOf<string>());
                Assert.That(responseArg.Data, Does.Contain("Kind Men"));
            })));

            var promise = uxios.Get(url, config);
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    if (responseAssertionException is not null) throw responseAssertionException;

                    Assert.That(interceptionIsCalled, Is.True);
                }
            );
        }

        [UnityTest]
        public IEnumerator InterceptRejectedResponses()
        {
            bool interceptionIsCalled = false;
            Uxios.Interceptors.response.Add(new(AssertErrorInterception(error =>
            {
                interceptionIsCalled = true;
                Assert.That(error.Response.Config, Is.TypeOf<Config>());

                // Double check whether the request was actually successfully completed
                Assert.That(error.Response.Status, Is.EqualTo(HttpStatusCode.NotFound));
            })));

            var promise = uxios.Get(new Uri("https://kind-men.com/404"), config);
            
            yield return Asserts.AssertPromiseErrors(
                promise,
                (exception) =>
                {
                    if (errorAssertionException is not null) throw errorAssertionException;

                    var error = exception as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error.Response, Is.Not.Null);
                    Assert.That(error.Response.Status, Is.EqualTo(HttpStatusCode.NotFound));

                    Assert.That(interceptionIsCalled, Is.True);
                }
            );
        }

        private RequestInterception AssertRequestInterception(Action<Config> assertions)
        {
            return request =>
            {
                try
                {
                    assertions(request);
                }
                catch (AssertionException e)
                {
                    requestAssertionException = e;
                }

                return request;
            };
        }

        private ResponseInterception AssertResponseInterception(Action<Response> assertions)
        {
            return response =>
            {
                try
                {
                    assertions(response);
                }
                catch (AssertionException e)
                {
                    responseAssertionException = e;
                }

                return response;
            };
        }

        private ErrorInterception AssertErrorInterception(Action<Error> assertions)
        {
            return error =>
            {
                try
                {
                    assertions(error);
                }
                catch (AssertionException e)
                {
                    errorAssertionException = e;
                }

                return error;
            };
        }
    }
}