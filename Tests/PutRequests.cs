using System;
using System.Collections;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests
{
    public class PutRequests
    {
        private class ExamplePost
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? id = null;

            public int userId;
            public string title;
            public string body;
        }

        [UnityTest]
        public IEnumerator PutsObjectAsJson()
        {
            var uxios = new KindMen.Uxios.Uxios();
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var data = new ExamplePost
            {
                id = 1, 
                userId = 1,
                title = "Example title", 
                body = "This is an example body"
            };

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                ExamplePost post = response.Data as ExamplePost;
                
                Assert.That(post, Is.Not.Null);
                Assert.That(post.id.Value, Is.EqualTo(1));
                Assert.That(post.userId, Is.EqualTo(data.userId));
                Assert.That(post.title, Is.EqualTo(data.title));
                Assert.That(post.body, Is.EqualTo(data.body));
                Assert.That(response.Headers, Contains.Key("Content-Type"));
                Assert.That(response.Headers["Content-Type"], Is.EqualTo("application/json; charset=utf-8"));
            };

            var promise = uxios.Put<ExamplePost, ExamplePost>(url, data);

            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }
    }
}