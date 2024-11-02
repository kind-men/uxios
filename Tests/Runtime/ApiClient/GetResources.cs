using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using UnityEngine.TestTools;
using QueryParameters = KindMen.Uxios.Http.QueryParameters;

namespace KindMen.Uxios.Tests.ApiClient
{
    public class GetResources
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
        public IEnumerator GetResource()
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

            var queryParameters = new QueryParameters{ { "userId", "1" } };
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

        private static List<ExamplePost> AssertReceivedCollectionOfPosts(Response response, int numberOfResults)
        {
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.TypeOf<List<ExamplePost>>());
            List<ExamplePost> posts = response.Data as List<ExamplePost>;
            Assert.That(posts, Is.Not.Null);
            Assert.That(posts, Has.Count.EqualTo(numberOfResults));
        
            return posts;
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