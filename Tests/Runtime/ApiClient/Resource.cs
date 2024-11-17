using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using KindMen.Uxios.Api;
using KindMen.Uxios.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.ApiClient
{
    public class Resource
    {
        private class ExamplePost
        {
            public int userId;
            public int id;
            public string title;
            public string body;
        }
        
        private class BearerAuthenticationSuccess
        {
            public bool authenticated;
            public string token;
        }

        private class BasicAuthenticationSuccess
        {
            public bool authenticated;
            public string user;
        }


        [UnityTest]
        public IEnumerator GetResource()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var resource = new Resource<ExamplePost>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(resource.Value, AssertCanonicalPost);
        }

        [UnityTest]
        public IEnumerator GetResourceUsingShortHand()
        {
            var resource = Resource<ExamplePost>.At(new Uri("https://jsonplaceholder.typicode.com/posts/1"));

            yield return PromiseAssertions.AssertPromiseSucceeds(resource.Value, AssertCanonicalPost);
        }

        [UnityTest]
        public IEnumerator GetResourceUsingShorterShortHand()
        {
            var resource = Resource<ExamplePost>.At("https://jsonplaceholder.typicode.com/posts/1");

            yield return PromiseAssertions.AssertPromiseSucceeds(resource.Value, AssertCanonicalPost);
        }

        [UnityTest]
        public IEnumerator GetResourceUsingUriTemplate()
        {
            var template = new TemplatedUri("https://jsonplaceholder.typicode.com/posts/{id}");

            var resource = Resource<ExamplePost>.At(template.With("id", "1").Uri());

            yield return PromiseAssertions.AssertPromiseSucceeds(resource.Value, AssertCanonicalPost);
        }

        [UnityTest]
        public IEnumerator CheckIfAValueExists()
        {
            var resource = Resource<ExamplePost>.At(new Uri("https://jsonplaceholder.typicode.com/posts/1"));

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.HasValue, 
                exists => Assert.That(exists, Is.True)
            );
        }

        [UnityTest]
        public IEnumerator CheckIfAValueDoesNotExists()
        {
            var resource = Resource<ExamplePost>.At(new Uri("https://httpbin.org/status/404"));

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.HasValue, 
                exists => Assert.That(exists, Is.False)
            );
        }

        [UnityTest]
        public IEnumerator ErrorWhenCheckingForAValuesExistenceThrowsAnUnexpectedError()
        {
            var resource = Resource<ExamplePost>.At(new Uri("https://httpbin.org/status/500"));

            yield return PromiseAssertions.AssertPromiseErrors(
                resource.HasValue, 
                e => Assert.That((e as Error)!.Response.Status, Is.EqualTo(HttpStatusCode.InternalServerError))
            );
        }

        [UnityTest]
        public IEnumerator GetResourceWithQueryParametersInUrl()
        {
            var url = new Uri("https://httpbin.org/anything?test=abc");

            var resource = Resource<HttpbinAnything>.At(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.Value,
                anything =>
                {
                    Assert.That(anything, Is.Not.Null);
                    Assert.That(anything.args, Contains.Key("test"));
                    Assert.That(anything.args["test"], Is.EqualTo("abc"));
                }
            );
        }

        [UnityTest]
        public IEnumerator GetResourceWithAddedQueryParameter()
        {
            var url = new Uri("https://httpbin.org/anything");

            var resource = Resource<HttpbinAnything>
                .At(url)
                .With("test", "abc");

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.Value,
                anything =>
                {
                    Assert.That(anything, Is.Not.Null);
                    Assert.That(anything.args, Contains.Key("test"));
                    Assert.That(anything.args["test"], Is.EqualTo("abc"));
                }
            );
        }

        [UnityTest]
        public IEnumerator GetResourceBehindBasicAuthentication()
        {
            var url = new Uri("https://httpbin.org/basic-auth/username/password");

            var resource = Resource<BasicAuthenticationSuccess>
                .At(url)
                .As("username", "password");

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.Value,
                basicAuthSuccess =>
                {
                    Assert.That(basicAuthSuccess, Is.Not.Null);
                    Assert.That(basicAuthSuccess.authenticated, Is.True);
                    Assert.That(basicAuthSuccess.user, Is.EqualTo("username"));
                }
            );
        }

        [UnityTest]
        public IEnumerator GetResourceBehindBearerAuthentication()
        {
            var url = new Uri("https://httpbin.org/bearer");

            var resource = Resource<BearerAuthenticationSuccess>
                .At(url)
                .As(new BearerTokenCredentials("Goldilocks"));

            yield return PromiseAssertions.AssertPromiseSucceeds(
                resource.Value,
                basicAuthSuccess =>
                {
                    Assert.That(basicAuthSuccess, Is.Not.Null);
                    Assert.That(basicAuthSuccess.authenticated, Is.True);
                    Assert.That(basicAuthSuccess.token, Is.EqualTo("Goldilocks"));
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

    public class HttpbinAnything
    {
        public Dictionary<string, string> args;
    }
}