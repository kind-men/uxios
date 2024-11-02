using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios.Tests
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
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        [UnityTest]
        public IEnumerator GetsWebpageAsStringUsingGenericShorthand()
        {
            var url = new Uri("https://httpbin.org/html");

            var promise = uxios.Get<string>(url);

            yield return Asserts.AssertPromiseSucceeds(
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

            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        [UnityTest]
        public IEnumerator GetsImageAsTexture()
        {
            var url = new Uri("https://httpbin.org/image/png");

            var promise = uxios.Get<Texture2D>(url);

            yield return Asserts.AssertPromiseSucceeds(
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
            
            yield return Asserts.AssertPromiseSucceeds(
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
        public IEnumerator CannotFindUrl()
        {
            var url = new Uri("https://httpbin.org/status/404");

            var promise = uxios.Get<string>(url);

            yield return Asserts.AssertPromiseErrors(
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
        public IEnumerator GetsJsonAsTypedObject()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var promise = uxios.Get<ExamplePost>(url);

            yield return Asserts.AssertPromiseSucceeds(
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

            yield return Asserts.AssertPromiseSucceeds(
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
        public IEnumerator GetsJsonAsCollectionOfObjects()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var promise = uxios.Get<List<ExamplePost>>(url);
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 100);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfObjectsUsingArguments()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var promise = uxios.Get<List<ExamplePost>>(url, new QueryParameters{ { "userId", "1" } });

            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfObjectsUsingQueryParametersOnTheUrl()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts?userId=1");

            var promise = uxios.Get<List<ExamplePost>>(url);

            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);

                    AssertCanonicalPost(posts.First());
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsFilteredCollectionOfObjectsUsingParametersInTheConfig()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var queryParameters = new QueryParameters{ { "userId", "1" } };
            var config = new Config { Params = queryParameters };
            var promise = uxios.Get<List<ExamplePost>>(url, config);
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    var posts = AssertReceivedCollectionOfPosts(response, 10);
                    AssertCanonicalPost(posts.First());
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

            yield return Asserts.AssertPromiseErrorsWithMessage(
                promise, 
                "Unable to parse response as JSON"
            );
        }

        private static List<ExamplePost> AssertReceivedCollectionOfPosts(Response response, int numberOfResults)
        {
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.TypeOf<List<ExamplePost>>());
            List<ExamplePost> posts = response.Data as List<ExamplePost>;
            Assert.That(posts, Is.Not.Null);
            Assert.That(posts, Has.Count.EqualTo(numberOfResults));
        
            return posts;
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