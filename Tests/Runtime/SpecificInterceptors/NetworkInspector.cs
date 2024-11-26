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

        [SetUp]
        public void SetUp()
        {
            uxios = new Uxios();
        }

        [UnityTest]
        public IEnumerator LogsGettingAWebpage()
        {
            // Auto-registers a series of interceptors, and deregisters once disposed
            using var logger = new ConsoleLogger();
            
            var url = new Uri("https://httpbin.org/html");
            var config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };

            var promise = uxios.Get(url, config);
            
            yield return PromiseAssertions.AssertPromiseSucceeds(
                promise, 
                AssertExampleHtmlWasReceived
            );
        }

        private void AssertExampleHtmlWasReceived(IResponse obj)
        {
            // TODO: Test whether the logger's calls were actually received - now we mainly test that the interceptors
            //  don't break the application
        }
    }
}