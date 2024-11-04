using System;
using System.Collections;
using KindMen.Uxios.Api;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.ApiClient
{
    public class UsingResourceProxy
    {
        private class ExamplePost
        {
            public int userId;
            public int id;
            public string title;
            public string body;
        }

        [UnityTest]
        public IEnumerator GetResource()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var resource = new Resource<ExamplePost>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.Value, 
                post =>
                {
                    AssertCanonicalPost(post as ExamplePost);
                }
            );
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