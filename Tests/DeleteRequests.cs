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
    public class DeleteRequests
    {
        [UnityTest]
        public IEnumerator DeletesResource()
        {
            var uxios = new Uxios();
            var url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            var promise = uxios.Delete(url);

            Action<Response> onSuccess = response =>
            {
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Data, Is.Empty);
            };

            yield return Asserts.AssertPromiseSucceeds(promise, onSuccess);
        }
    }
}