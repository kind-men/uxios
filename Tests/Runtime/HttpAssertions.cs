using System.Net;
using NUnit.Framework;

namespace KindMen.Uxios.Tests
{
    public static class HttpAssertions
    {
        internal static void AssertStatusCode(IResponse response, HttpStatusCode httpStatusCode)
        {
            Assert.That(response.Status, Is.EqualTo(httpStatusCode));
        }
    }
}