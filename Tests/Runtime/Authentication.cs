using System;
using System.Collections;
using System.IO;
using System.Net;
using KindMen.Uxios.Errors.Http;
using KindMen.Uxios.Http;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests
{
    public class Authentication
    {
        private struct BearerAuthenticationSuccess
        {
            public bool authenticated;
            public string token;
        }

        private Uxios uxios;

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator GetsWebpageBehindBasicAuthentication()
        {
            var url = new Uri("https://httpbin.org/basic-auth/username/password");

            var credentials = new BasicAuthenticationCredentials("username", "password");
            var config = new Config { Auth = credentials };
            var promise = uxios.Get(url, config);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise,
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
    
                    var json = response.Data as JObject;
                    Assert.That(json, Is.Not.Null);
                    Assert.That(json["authenticated"]?.Value<bool>(), Is.True);
                    Assert.That(json["user"]?.Value<string>(), Is.EqualTo("username"));
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfBasicAuthenticationFails()
        {
            var url = new Uri("https://httpbin.org/basic-auth/username/password");

            var credentials = new BasicAuthenticationCredentials("username", "wrong-password");
            var config = new Config { Auth = credentials };
            var promise = uxios.Get(url, config);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise,
                exception =>
                {
                    Error error = exception as Error;
                    var response = error.Response;
                    HttpAssertions.AssertStatusCode(response, HttpStatusCode.Unauthorized);
    
                    Assert.That(response.Data, Is.Null);
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfBasicAuthenticationIsNotProvidedWhileNeeded()
        {
            var url = new Uri("https://httpbin.org/basic-auth/username/password");
            var promise = uxios.Get<FileInfo>(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise,
                exception =>
                {
                    AuthenticationError error = exception as AuthenticationError;
                    Assert.That(error, Is.InstanceOf(typeof(AuthenticationError)));
                    Assert.That(error, Is.TypeOf(typeof(UnauthorizedError)));
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfTokenBasedAuthenticationIsNotProvided()
        {
            var url = new Uri("https://tile.googleapis.com/v1/3dtiles/root.json");
            var promise = uxios.Get<FileInfo>(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise,
                exception =>
                {
                    AuthenticationError error = exception as AuthenticationError;
                    Assert.That(error, Is.InstanceOf(typeof(AuthenticationError)));
                    Assert.That(error, Is.TypeOf(typeof(ForbiddenError)));
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsApiResponseBehindBearerAuthentication()
        {
            var url = new Uri("https://httpbin.org/bearer");

            var credentials = new BearerTokenCredentials("Goldilocks");
            var config = new Config { Auth = credentials };
            var promise = uxios.Get<BearerAuthenticationSuccess>(url, config);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise,
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
    
                    BearerAuthenticationSuccess json = (BearerAuthenticationSuccess)response.Data;
                    Assert.That(json, Is.Not.Null);
                    Assert.That(json.authenticated, Is.True);
                    Assert.That(json.token, Is.EqualTo("Goldilocks"));
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfBearerAuthenticationFails()
        {
            var url = new Uri("https://httpbin.org/bearer");

            // Omit bearer token / credentials; this should fail
            // TODO: I couldn't use the BearerAuthenticationSuccess type hint because Response
            //    would become null in a failure. I should figure out why, because being unable
            //    to have a response is unhelpful because it cannot be inspected anymore
            //    As soon as this is fixed (see test in GetRequests), we omit the type here.
            //    Reintroduce the type as soon as it is fixed
            var promise = uxios.Get(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise,
                exception =>
                {
                    var response = (exception as Error).Response;
                    Assert.That(response, Is.Not.Null);
                    HttpAssertions.AssertStatusCode(response, HttpStatusCode.Unauthorized);
    
                    Assert.That(response.Data, Is.Null);
                }
            );
        }
    }
}