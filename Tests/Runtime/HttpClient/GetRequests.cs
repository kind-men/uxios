using System;
using System.Collections;
using System.Net;
using KindMen.Uxios.Http;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.HttpClient
{
    public class GetRequests
    {
        private Uxios uxios;

        private class ExamplePost
        {
            public int userId;
            public int id;
            public string title;
            public string body;
        }

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator GetsWebpageAsString()
        {
            var url = new Uri("https://httpbin.org/html");
            var config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };

            var promise = uxios.Get(url, config);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        [UnityTest]
        public IEnumerator GetsWebpageAsStringUsingGenericShorthand()
        {
            var url = new Uri("https://httpbin.org/html");

            var promise = uxios.Get<string>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
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
        public IEnumerator GetsWebpageBasedOnRelativeUrl()
        {
            var config = new Config { BaseUrl = new Uri("https://httpbin.org") };
            var relativeUrl = new Uri("html", UriKind.Relative);

            var promise = uxios.Get<string>(relativeUrl, config);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        [UnityTest]
        public IEnumerator GetsImageAsTexture()
        {
            var url = new Uri("https://httpbin.org/image/png");

            var promise = uxios.Get<Texture2D>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<Texture2D>());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsImageAsByteArray()
        {
            var url = new Uri("https://httpbin.org/image/png");

            var promise = uxios.Get<byte[]>(url);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<byte[]>());
                    Assert.That(response.Data, Is.Not.Empty);
                }
            );
        }

        // TODO: Add more error scenario's, such as CORS, DNS not found, timeout (difficult one to test) and 
        //       a Unity DataProcessingError (trying to load a JSON as a Texture I think); use https://httpbin.org/
        [UnityTest]
        public IEnumerator ErrorIfItCannotFindUrl()
        {
            var url = new Uri("https://httpbin.org/status/404");

            var promise = uxios.Get<string>(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise, 
                e =>
                {
                    var error = e as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error.Response, Is.Not.Null);
                    Assert.That(error.Response.Status, Is.EqualTo(HttpStatusCode.NotFound));
                }
            );
        }
        
        [UnityTest]
        public IEnumerator ErrorIfTimeoutExpires()
        {
            var url = new Uri("https://httpbin.org/delay/3");

            // Time-out set to 1 while the delay above is 3, this should trigger a timeout
            Config config = new Config() { Timeout = 1 };
            var promise = uxios.Get<string>(url, config);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise, 
                e =>
                {
                    var error = e as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error.Response, Is.Null);
                    Assert.That(error.Message, Is.EqualTo("Request timeout"));
                }
            );
        }
        
        [UnityTest]
        public IEnumerator ErrorIfDurationExceedsTheNumberOfRedirects()
        {
            var url = new Uri("https://httpbin.org/redirect/5");

            // Time-out set to 3 while the delay above is 5, this should trigger an error
            Config config = new Config() { MaxRedirects = 3 };
            var promise = uxios.Get<string>(url, config);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise, 
                e =>
                {
                    var error = e as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error.Response, Is.Null);
                    Assert.That(error.Message, Is.EqualTo("Redirect limit exceeded"));
                }
            );
        }
        
        [UnityTest]
        public IEnumerator ErrorIfHostDoesNotExist()
        {
            var url = new Uri("https://thisdomaindoesnotexist.eu/");

            var promise = uxios.Get<string>(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise, 
                e =>
                {
                    var error = e as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error.Response, Is.Null);
                    Assert.That(error.Message, Is.EqualTo("Cannot resolve destination host"));
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfAuthenticationFails()
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
        public IEnumerator GetsJsonAsTypedObject()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var promise = uxios.Get<ExamplePost>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                    AssertCanonicalPost(response.Data as ExamplePost);
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsJsonAsUntypedObject()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            // No indication of a return type means: untyped JSON.
            var promise = uxios.Get(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<JObject>());
                    JObject post = response.Data as JObject;
                    Assert.That(post, Is.Not.Null);
                    Assert.That(post.GetValue("id")?.Value<int>(), Is.EqualTo(1));
                    Assert.That(post["userId"]?.Value<int>(), Is.EqualTo(1));
                    Assert.That(post["title"]?.Value<string>(), Is.EqualTo("sunt aut facere repellat provident occaecati excepturi optio reprehenderit"));
                    Assert.That(post["body"]?.Value<string>(), Is.EqualTo("quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"));
                }
            );
        }

        [UnityTest]
        public IEnumerator ErrorIfResponseCannotBeInterpretedAsJson()
        {
            // Start with a URL that returns an HTML response ... 
            var url = new Uri("https://httpbin.org/html");

            // ... and thus should fail because JSON is expected due to the use of a Generic
            var promise = uxios.Get<ExamplePost>(url);

            yield return PromiseAssertions.AssertPromiseErrorsWithMessage(
                promise, 
                "Unable to parse response as JSON"
            );
        }

        private void AssertExampleHtmlWasReceived(Response response)
        {
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.TypeOf<string>());
            Assert.That(response.Data, Does.Contain("Herman Melville"));
        }

        private static void AssertCanonicalPost(ExamplePost post)
        {
            Assert.That(post, Is.Not.Null);
            Assert.That(post.id, Is.EqualTo(1));
            Assert.That(post.userId, Is.EqualTo(1));
            Assert.That(post.title, Is.EqualTo("sunt aut facere repellat provident occaecati excepturi optio reprehenderit"));
            Assert.That(post.body, Is.EqualTo("quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"));
        }
    }
}