using System;
using System.Collections;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.PersistentDataClient
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
        public IEnumerator PostsObjectAsJsonToFileInRoot()
        {
            string fileName = "1.json";
            var url = new Uri("unity+persistent:///" + fileName);

            var data = new ExamplePost
            {
                userId = 10, 
                title = "Example title", 
                body = "This is an example body"
            };

            var promise = uxios.Post<ExamplePost, byte[]>(url, data);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    // Posting a file always returns OK instead of Created because POST and PUT are treated equally
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    
                    // Posting a file does not yield data back
                    Assert.That(response.Data, Is.Empty);
                    var path = Path.Combine(Application.persistentDataPath, fileName);
                    Assert.That(File.Exists(path), Is.True);
                    Assert.That(File.ReadAllText(path), Is.EqualTo("{\"userId\":10,\"title\":\"Example title\",\"body\":\"This is an example body\"}"));
                }
            );
        }

        [UnityTest]
        public IEnumerator PostsObjectAsJsonToFileInSubdirectory()
        {
            string fileName = "subfolder/1.json";
            var url = new Uri("unity+persistent:///" + fileName);

            var data = new ExamplePost
            {
                userId = 10, 
                title = "Example title", 
                body = "This is an example body"
            };

            var promise = uxios.Post<ExamplePost, byte[]>(url, data);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    // Posting a file always returns OK instead of Created because POST and PUT are treated equally
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    
                    // Posting a file does not yield data back
                    Assert.That(response.Data, Is.Empty);
                    var path = Path.Combine(Application.persistentDataPath, fileName);
                    Assert.That(File.Exists(path), Is.True);
                    Assert.That(File.ReadAllText(path), Is.EqualTo("{\"userId\":10,\"title\":\"Example title\",\"body\":\"This is an example body\"}"));
                }
            );
        }
    }
}