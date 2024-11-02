using System;
using System.Collections;
using System.Net;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.HttpClient
{
    public class DeleteRequests
    {
        private Uxios uxios;

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator DeletesResource()
        {
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var promise = uxios.Delete(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    HttpAssertions.AssertStatusCode(response, HttpStatusCode.OK);
                    Assert.That(response.Data, Is.Empty);
                }
            );
        }
    }
}