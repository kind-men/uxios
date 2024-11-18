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
    public class GetRequests
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
        public IEnumerator GetObjectAsJson()
        {
            string fileName = "1.json";
            var url = new Uri("unity+persistent:///" + fileName);

            // Arrange for a file to be there
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, "{\"userId\":10,\"title\":\"Example title\",\"body\":\"This is an example body\"}");
            
            var data = new ExamplePost
            {
                userId = 10, 
                title = "Example title", 
                body = "This is an example body"
            };

            var promise = uxios.Get<ExamplePost>(url);

            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                response =>
                {
                    Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                    
                    Assert.That(response.Data, Is.TypeOf<ExamplePost>());
                    
                    int Comparison(ExamplePost post, ExamplePost examplePost)
                    {
                        return post != null 
                            && examplePost != null 
                            && post.userId == examplePost.userId
                            && post.body == examplePost.body
                            && post.id == examplePost.id
                            && post.title == examplePost.title ? 0 : 1;
                    }

                    Assert.That(response.Data, Is.EqualTo(data).Using<ExamplePost>(Comparison));
                }
            );
        }
    }
}