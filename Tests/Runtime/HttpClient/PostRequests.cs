using System;
using System.Collections;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.HttpClient
{
    public class PostRequests
    {
        private Uxios uxios;

        private class ExamplePost
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? id = null;

            public int userId;
            public string title;
            public string body;
        }

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator PostsObjectAsJson()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts");

            var data = new ExamplePost
            {
                userId = 10, 
                title = "Example title", 
                body = "This is an example body"
            };

            var promise = uxios.Post<ExamplePost, ExamplePost>(url, data);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.Created));
                    Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                    
                    ExamplePost post = response.Data as ExamplePost;
                
                    Assert.That(post, Is.Not.Null);
                    Assert.That(post.id.Value, Is.EqualTo(101));
                    Assert.That(post.userId, Is.EqualTo(data.userId));
                    Assert.That(post.title, Is.EqualTo(data.title));
                    Assert.That(post.body, Is.EqualTo(data.body));
                    Assert.That(response.Headers, Contains.Key("Content-Type"));
                    Assert.That(response.Headers["Content-Type"], Is.EqualTo("application/json; charset=utf-8"));
                }
            );
        }
    }
}