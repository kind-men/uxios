using System;
using System.Collections;
using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests
{
    public class Uxios
    {
        private class ExamplePost
        {
            public int userId;
            public int id;
            public string title;
            public string body;
        }

        [UnityTest]
        public IEnumerator GetsWebpageAsString()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com");
            var config = new Config { ResponseType = ExpectedResponse.Text() };

            var promise = uxios.Get(url, config);
            
            yield return Asserts.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<string>());
                    Assert.That(response.Data, Does.Contain("Kind Men"));
                }
            );
        }

        [UnityTest]
        public IEnumerator GetsWebpageAsStringUsingGenericShorthand()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com");

            var promise = uxios.Get<string>(url);

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<string>());
                Assert.That(response.Data, Does.Contain("Kind Men"));
            };

            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }

        [UnityTest]
        public IEnumerator GetsWebpageBasedOnRelativeUrl()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var config = new Config { BaseUrl = new Uri("https://kind-men.com") };
            var relativeUrl = new Uri("2016/01/22/launching-our-new-website/", UriKind.Relative);

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<string>());
                Assert.That(response.Data, Does.Contain("Kind Men"));
            };

            var promise = uxios.Get<string>(relativeUrl, config);

            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }

        [UnityTest]
        public IEnumerator GetsImageAsTexture()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com/wp-content/uploads//sites/7/2020/06/us.png");

            var promise = uxios.Get<Texture2D>(url);

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<Texture2D>());
            };
            
            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }

        [UnityTest]
        public IEnumerator GetsImageAsByteArray()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com/wp-content/uploads//sites/7/2020/06/us.png");

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<byte[]>());
                Assert.That(response.Data, Is.Not.Empty);
            };

            var promise = uxios.Get<byte[]>(url);
            
            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }

        [UnityTest]
        public IEnumerator CannotFindUrl()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com/404");

            Action<Exception> onError = e =>
            {
                var error = e as Error;
                Assert.That(error, Is.Not.Null);
                Assert.That(error.Response.Status, Is.EqualTo(HttpStatusCode.NotFound));
            };

            var promise = uxios.Get<string>(url);

            yield return Asserts.AssertPromiseErrors(promise, onError);
        }
        
        [UnityTest]
        public IEnumerator GetsJsonAsTypedObject()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                ExamplePost post = response.Data as ExamplePost;
                Assert.That(post, Is.Not.Null);
                Assert.That(post.id, Is.EqualTo(1));
                Assert.That(post.userId, Is.EqualTo(1));
                Assert.That(post.title, Is.EqualTo("sunt aut facere repellat provident occaecati excepturi optio reprehenderit"));
                Assert.That(post.body, Is.EqualTo("quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"));
            };

            var promise = uxios.Get<ExamplePost>(url);
            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }

        [UnityTest]
        public IEnumerator GetsJsonAsUntypedObject()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<JObject>());
                JObject post = response.Data as JObject;
                Assert.That(post, Is.Not.Null);
                Assert.That(post.GetValue("id")?.Value<int>(), Is.EqualTo(1));
                Assert.That(post["userId"]?.Value<int>(), Is.EqualTo(1));
                Assert.That(post["title"]?.Value<string>(), Is.EqualTo("sunt aut facere repellat provident occaecati excepturi optio reprehenderit"));
                Assert.That(post["body"]?.Value<string>(), Is.EqualTo("quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"));
            };

            // No indication of a return type means: untyped JSON.
            var promise = uxios.Get(url);
            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }
        
        // TODO: Add case and solution for query parameters / https://jsonplaceholder.typicode.com/guide/#filtering-resources

        [UnityTest]
        public IEnumerator ErrorIfResponseCannotBeInterpretedAsJson()
        {
            var uxios = new KindMen.Uxios.Uxios();
            
            // Start with a URL that returns an HTML response ... 
            var url = new Uri("https://kind-men.com");

            // ... and thus should fail because JSON is expected due to the use of a Generic
            var promise = uxios.Get<ExamplePost>(url);

            yield return Asserts.AssertPromiseErrorsWithMessage(promise, "Unable to parse response as JSON");
        }

        [UnityTest]
        public IEnumerator PostsObjectAsJson()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://kind-men.com");

            var data = new ExamplePost
            {
                userId = 10, 
                id = 11, 
                title = "Example title", 
                body = "This is an example body"
            };

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                ExamplePost post = response.Data as ExamplePost;
                
                Assert.That(post, Is.Not.Null);
                Assert.That(post.userId, Is.EqualTo(data.userId));
                Assert.That(post.id, Is.EqualTo(data.id));
                Assert.That(post.title, Is.EqualTo(data.title));
                Assert.That(post.body, Is.EqualTo(data.body));
                Assert.That(response.Headers, Contains.Key("Content-type"));
                Assert.That(response.Headers["Content-type"], Is.EqualTo("application/json; charset=UTF-8"));
            };

            var promise = uxios.Post<ExamplePost, ExamplePost>(url, data);
            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }
    }
}