using NUnit.Framework;
using System.Collections.Specialized;

namespace KindMen.Uxios.Tests
{
    public class QueryStringTests
    {
        [Test]
        public void SpaceIsEscapedToPlus()
        {
            string input = "hello world";
            string expected = "hello+world";

            string result = QueryString.Escape(input);
            
            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void SpecialCharactersAreEscaped()
        {
            string input = "hello+world";
            string expected = "hello%2Bworld";

            string result = QueryString.Escape(input);

            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void PlusIsUnescapedToSpace()
        {
            string input = "hello+world";
            string expected = "hello world";

            string result = QueryString.Unescape(input);

            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void EscapedSpecialCharactersAreUnescaped()
        {
            string input = "hello%2Bworld";
            string expected = "hello+world";

            string result = QueryString.Unescape(input);
            
            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void EncodingASingleKeyValuePair()
        {
            var collection = new NameValueCollection
            {
                { "key", "value" }
            };
            string expected = "key=value";

            string result = QueryString.Encode(collection);

            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void EncodingMultipleKeyValuePairs()
        {
            var collection = new NameValueCollection
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            string expected = "key1=value1&key2=value2";
            
            string result = QueryString.Encode(collection);

            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void EncodingArrayLikeKeys()
        {
            var collection = new NameValueCollection
            {
                { "arrayKey[]", "value1" },
                { "arrayKey[]", "value2" }
            };
            string expected = "arrayKey[]=value1&arrayKey[]=value2";

            string result = QueryString.Encode(collection);
            
            Assert.That(expected, Is.EqualTo(result));
        }

        [Test]
        public void EncodingAnEmptyCollectionReturnsEmptyString()
        {
            var collection = new NameValueCollection();
            
            string result = QueryString.Encode(collection);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DecodingASingleKeyValuePair()
        {
            string input = "key=value";
            
            var result = QueryString.Decode(input);
            
            Assert.That("value", Is.EqualTo(result["key"]));
        }

        [Test]
        public void DecodingMultipleKeyValuePairs()
        {
            string input = "key1=value1&key2=value2";

            var result = QueryString.Decode(input);
            
            Assert.That("value1", Is.EqualTo(result["key1"]));
            Assert.That("value2", Is.EqualTo(result["key2"]));
        }

        [Test]
        public void DecodingArrayLikeKeys()
        {
            string input = "arrayKey[]=value1&arrayKey[]=value2";
            
            var result = QueryString.Decode(input);
            
            Assert.That(result.GetValues("arrayKey"), Has.Length.EqualTo(2));
            Assert.That("value1", Is.EqualTo(result.GetValues("arrayKey")[0]));
            Assert.That("value2", Is.EqualTo(result.GetValues("arrayKey")[1]));
        }

        [Test]
        public void DecodingAnEmptyQueryStringReturnsEmptyCollection()
        {
            string input = "";

            var result = QueryString.Decode(input);
            
            Assert.That(result, Is.Empty);
        }
    }
}