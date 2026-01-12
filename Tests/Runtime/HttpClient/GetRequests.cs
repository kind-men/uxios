using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using KindMen.Uxios.Errors;
using KindMen.Uxios.Errors.Connection;
using KindMen.Uxios.Http;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

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
        public IEnumerator GetsWebpageAsStringWithPayload()
        {
            var url = new Uri("https://httpbin.org/html");
            var config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };
            config.WithPayload(new ExamplePost()
            {
                id = 123,
                title = "This is a test"
            });

            var promise = uxios.Get(url, config);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    
                    var payload = response.Config.GetPayload<ExamplePost>();
                    Assert.That(payload, Is.TypeOf<ExamplePost>());
                    Assert.That(payload.id, Is.EqualTo(123));
                    Assert.That(payload.title, Is.EqualTo("This is a test"));
                }
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
        public IEnumerator GetsFileAsDownloadedFileInfoReference()
        {
            var url = new Uri("https://kind-men.github.io/uxios/images/logo.png");

            var promise = uxios.Get<FileInfo>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    var responseData = response.Data as FileInfo;
                    
                    Assert.That(responseData, Is.Not.Null);
                    Assert.That(responseData.Length, Is.GreaterThan(0));
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

            // Time-out set to 1s while the delay above is 3s, this should trigger a timeout
            Config config = new Config() { Timeout = 1000 };
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
        public IEnumerator CancelARequest()
        {
            var url = new Uri("https://httpbin.org/delay/10");

            var promise = uxios.Get<string>(url);

            yield return new WaitForSeconds(1);
            uxios.Abort(promise);
            
            yield return PromiseAssertions.AssertPromiseErrors(
                promise, 
                e =>
                {
                    var error = e as Error;
                    Assert.That(error, Is.Not.Null);
                    Assert.That(error, Is.InstanceOf<ConnectionError>());
                    Assert.That(error, Is.InstanceOf<RequestAbortedError>());
                    Assert.That(error.Response, Is.Null);
                    Assert.That(error.Message, Is.EqualTo("Request aborted"));
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
        public IEnumerator UseUriTemplateToResolveDynamicUriParts()
        {
            var url = new TemplatedUri("https://jsonplaceholder.typicode.com/posts/{id}");

            var promise = uxios.Get<ExamplePost>(url.With("id", "1"));

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Request.Url.ToString(), Is.EqualTo("https://jsonplaceholder.typicode.com/posts/1"));
                    Assert.That(response.Request.QueryString, Is.Empty);
                    Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                    AssertCanonicalPost(response.Data as ExamplePost);
                }
            );
        }

        [UnityTest]
        public IEnumerator UriTemplatesWillConsumeQueryParametersWhenPartsAreUnresolved()
        {
            var url = new TemplatedUri("https://jsonplaceholder.typicode.com/posts/{id}");

            // Add a param, independently of the provided template
            var config = new Config().AddParam("id", "1");
            
            var promise = uxios.Get<ExamplePost>(url.Using(config.Params), config);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Request.Url.ToString(), Is.EqualTo("https://jsonplaceholder.typicode.com/posts/1"));
                    Assert.That(response.Request.QueryString, Is.Empty);
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

        private struct BearerAuthenticationSuccess
        {
            public bool authenticated;
            public string token;
        }

        [UnityTest]
        public IEnumerator AnErrorShouldAllowForAResponseToBeAvailable()
        {
            var url = new Uri("https://httpbin.org/bearer");

            // Omit bearer token / credentials; this should fail; the response should be available
            // but Data would be as it cannot be parsed - or a NullValue object with the raw data?
            var promise = uxios.Get<BearerAuthenticationSuccess>(url);

            yield return PromiseAssertions.AssertPromiseErrors(
                promise,
                exception =>
                {
                    var response = (exception as Error).Response;
                    Assert.That(response, Is.Not.Null);
                    HttpAssertions.AssertStatusCode(response, HttpStatusCode.Unauthorized);
    
                    Assert.That(response.Data, Is.Empty);
                }
            );
        }

                [UnityTest]
        public IEnumerator GetCollectionOfResources()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var promise = uxios.Get<List<ExamplePost>>(url);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 100);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfResources()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var promise = uxios.Get<List<ExamplePost>>(url, new QueryParameters{ { "userId", "1" } });

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfResourcesUsingQueryParametersInTheUrl()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts?userId=1");

            var promise = uxios.Get<List<ExamplePost>>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfResourcesUsingParametersInTheConfig()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var queryParameters = new QueryParameters() { { "userId", "1" } };
            var config = new Config { Params = queryParameters };
            var promise = uxios.Get<List<ExamplePost>>(url, config);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);
                    AssertCanonicalPost(posts.First());
                }
            );
        }

        private static List<ExamplePost> AssertReceivedCollectionOfPosts(IResponse response, int numberOfResults)
        {
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.TypeOf<List<ExamplePost>>());
            List<ExamplePost> posts = response.Data as List<ExamplePost>;
            Assert.That(posts, Is.Not.Null);
            Assert.That(posts, Has.Count.EqualTo(numberOfResults));
        
            return posts;
        }

        private void AssertExampleHtmlWasReceived(IResponse response)
        {
            Assert.That(response.Request.Headers.ContainsKey(Headers.ContentType), Is.False);
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