namespace KindMen.Uxios.Tests.Http
{
    using NUnit.Framework;
    using KindMen.Uxios.Http;

    public class QueryParametersTests
    {
        [Test]
        public void ConstructEmptyCollectionOfParameters()
        {
            var queryParams = new QueryParameters();

            Assert.That(queryParams.Count, Is.EqualTo(0));
        }

        [Test]
        public void ConstructWithExistingCollection()
        {
            var initialParams = new QueryParameters();
            initialParams.Add("key1", "value1");
            initialParams.Add("key2", "value2");

            var queryParams = new QueryParameters(initialParams);

            Assert.That(queryParams.Count, Is.EqualTo(2));
            Assert.That(queryParams["key1"].Single, Is.EqualTo("value1"));
            Assert.That(queryParams["key2"].Single, Is.EqualTo("value2"));
        }

        [Test]
        public void ConstructWithQueryString()
        {
            string queryString = "key1=value1&key2=value2";

            var queryParams = new QueryParameters(queryString);

            Assert.That(queryParams.Count, Is.EqualTo(2));
            Assert.That(queryParams["key1"].Single, Is.EqualTo("value1"));
            Assert.That(queryParams["key2"].Single, Is.EqualTo("value2"));
        }

        [Test]
        public void ConstructWithArrayLikeKeys()
        {
            string queryString = "arrayKey[]=value1&arrayKey[]=value2";

            var queryParams = new QueryParameters(queryString);

            Assert.That(queryParams.Count, Is.EqualTo(1));
            Assert.That(queryParams.GetValues("arrayKey").Length, Is.EqualTo(2));
            Assert.That(queryParams.GetValues("arrayKey"), Is.EquivalentTo(new[] { "value1", "value2" }));
        }

        [Test]
        public void ConvertToStringWithSingleQueryParameter()
        {
            var queryParams = new QueryParameters();
            queryParams.Add("key", "value");

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo("key=value"));
        }

        [Test]
        public void ConvertToStringWithEmptySingleQueryParameter()
        {
            var queryParams = new QueryParameters();
            queryParams.Add("key", string.Empty);

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo("key="));
        }

        [Test]
        public void ConvertToStringWithNullSingleQueryParameter()
        {
            var queryParams = new QueryParameters();
            queryParams.Add("key", null);

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo("key="));
        }

        [Test]
        public void ConvertToStringWithMultipleQueryParameters()
        {
            var queryParams = new QueryParameters();
            queryParams.Add("key1", "value1");
            queryParams.Add("key2", "value2");

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo("key1=value1&key2=value2"));
        }

        [Test]
        public void ConvertToStringWithArrayLikeSet()
        {
            var queryParams = new QueryParameters();
            queryParams.Add("arrayKey[]", "value1");
            queryParams.Add("arrayKey[]", "value2");

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo("arrayKey[]=value1&arrayKey[]=value2"));
        }

        [Test]
        public void ConvertingEmptyCollectionReturnsEmptyString()
        {
            var queryParams = new QueryParameters();

            string result = queryParams.ToString();

            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}