using System;
using System.Collections;
using KindMen.Uxios.Interceptors.NetworkInspector;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests.SpecificInterceptors
{
    public class NetworkInspector
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
        public IEnumerator GetsWebpageAsString()
        {
            // Auto-registers a series of interceptors (not sure whether this is the right solution yet)
            var logger = new Logger();
            
            var url = new Uri("https://httpbin.org/html");
            var config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };

            var promise = uxios.Get(url, config);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        private void AssertExampleHtmlWasReceived(Response obj)
        {
            
        }
    }
}