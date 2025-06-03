using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using KindMen.Uxios.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

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

        private class HttpBinPostResponse
        {
            public string data;
        }

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator PostsRawData()
        {
            var url = new Uri("https://httpbin.org/post");

            var data = new byte[1024];
            Random random = new Random();
            random.NextBytes(data);

            var promise = uxios.Post<byte[], HttpBinPostResponse>(url, data);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Request.Headers, Contains.Key(Headers.ContentType));
                    Assert.That(response.Request.Headers[Headers.ContentType], Is.EqualTo("application/octet-stream"));

                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Data, Is.TypeOf<HttpBinPostResponse>());
                    
                    HttpBinPostResponse post = response.Data as HttpBinPostResponse;
                
                    var expectedData = "data:application/octet-stream;base64," + Convert.ToBase64String(data);
    
                    Assert.That(post, Is.Not.Null);
                    Assert.That(post.data, Is.EqualTo(expectedData));
                    Assert.That(response.Headers, Contains.Key(Headers.ContentType));
                    Assert.That(response.Headers[Headers.ContentType], Is.EqualTo("application/json"));
                }
            );
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
                    Assert.That(response.Request.Headers, Contains.Key(Headers.ContentType));
                    Assert.That(response.Request.Headers[Headers.ContentType], Is.EqualTo("application/json"));
                    
                    ExamplePost post = response.Data as ExamplePost;
                
                    Assert.That(post, Is.Not.Null);
                    Assert.That(post.id.Value, Is.EqualTo(101));
                    Assert.That(post.userId, Is.EqualTo(data.userId));
                    Assert.That(post.title, Is.EqualTo(data.title));
                    Assert.That(post.body, Is.EqualTo(data.body));
                    Assert.That(response.Headers, Contains.Key(Headers.ContentType));
                    Assert.That(response.Headers[Headers.ContentType], Is.EqualTo("application/json; charset=utf-8"));
                }
            );
        }
    }
}