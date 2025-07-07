using NUnit.Framework;
using KindMen.Uxios.Http;

namespace KindMen.Uxios.Tests
{
    public class QueryStringTests
    {
        public class TestObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

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
            string expected = "hello%2bworld";

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
            var collection = new QueryParameters
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
            var collection = new QueryParameters
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
            var collection = new QueryParameters
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
            var collection = new QueryParameters();
            
            string result = QueryString.Encode(collection);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DecodingASingleKeyValuePair()
        {
            string input = "key=value";
            
            var result = QueryString.Decode(input);
            
            Assert.That("value", Is.EqualTo(result["key"].Values[0]));
        }

        [Test]
        public void DecodingMultipleKeyValuePairs()
        {
            string input = "key1=value1&key2=value2";

            var result = QueryString.Decode(input);
            
            Assert.That("value1", Is.EqualTo(result["key1"].Values[0]));
            Assert.That("value2", Is.EqualTo(result["key2"].Values[0]));
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

        [Test]
        public void SerializingAnObjectShouldReturnValidQueryString()
        {
            // Arrange
            var testObject = new TestObject
            {
                Name = "John Doe",
                Age = 30,
                IsActive = true
            };

            // Act
            string queryString = QueryString.Serialize(testObject);

            // Assert
            Assert.That(queryString, Is.EqualTo("Name=John+Doe&Age=30&IsActive=true"));
        }

        [Test]
        public void DeserializingAStringShouldPopulateObjectCorrectly()
        {
            // Arrange
            string queryString = "Name=John%20Doe&Age=30&IsActive=true";

            // Act
            var result = QueryString.Deserialize<TestObject>(queryString);

            // Assert
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual(30, result.Age);
            Assert.IsTrue(result.IsActive);
        }

        [Test]
        public void DeserializingShouldTreatKeysCaseInsensitive()
        {
            // Arrange
            string queryString = "name=John%20Doe&age=30&isActive=true";

            // Act
            var result = QueryString.Deserialize<TestObject>(queryString);

            // Assert
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual(30, result.Age);
            Assert.IsTrue(result.IsActive);
        }

        [Test]
        public void SerializeAndDeserializeShouldPreserveData()
        {
            // Arrange
            var originalObject = new TestObject
            {
                Name = "Jane Doe",
                Age = 25,
                IsActive = false
            };

            // Act
            string queryString = QueryString.Serialize(originalObject);
            var deserializedObject = QueryString.Deserialize<TestObject>(queryString);

            // Assert
            Assert.AreEqual(originalObject.Name, deserializedObject.Name);
            Assert.AreEqual(originalObject.Age, deserializedObject.Age);
            Assert.AreEqual(originalObject.IsActive, deserializedObject.IsActive);
        }
    }
}