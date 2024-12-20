﻿using System;
using System.Collections.Generic;
using KindMen.Uxios.Http;
using NUnit.Framework;

namespace KindMen.Uxios.Tests
{
    public class TemplatedUriTests
    {
        [Test]
        public void InitializesWithUriStringButNoParams()
        {
            var templatedUri = new TemplatedUri("https://example.com/users/{userId}/posts/{postId}");
            Assert.AreEqual("https://example.com/users/{userId}/posts/{postId}", templatedUri.ToString());
        }

        [Test]
        public void ExtractsCorrectParts()
        {
            var templatedUri = new TemplatedUri(
                "https://example.com/users/{userId}/posts/{postId}/comments/{commentId}"
            );
            var parts = templatedUri.GetUriTemplateParts();

            var expectedParts = new List<string> { "userId", "postId", "commentId" };
            
            CollectionAssert.AreEquivalent(expectedParts, parts);
        }

        [Test]
        public void ReplacesTemplatePartsWithQueryParameters()
        {
            var parameters = new QueryParameters();
            parameters.Set("userId", "123");
            parameters.Set("postId", "456");

            var templatedUri = new TemplatedUri("https://example.com/users/{userId}/posts/{postId}", parameters);
            var resolvedUri = templatedUri.ToString();

            // The used parameters should be removed as they are 'consumed'
            Assert.That(parameters, Is.Empty);
            Assert.AreEqual("https://example.com/users/123/posts/456", resolvedUri);
        }

        [Test]
        public void ReplacesTemplatePartsWithQueryParametersAddedLater()
        {
            // TODO: this test fails because parameters is newly created in the With function - and thus the original
            // was not modified. This is interesting because on the one hand we want to consume values, but on the other
            // hand UriTemplate should be used as a template and the With method should not alter a templated version
            // of parameters.. I need to think some more on what direction to take this
            var parameters = new QueryParameters {{"userId", "123"}};
            var templatedUri = new TemplatedUri("https://example.com/users/{userId}/posts/{postId}", parameters);
            
            // Add postId after the fact to make it easier to work with templates
            var appendedUri = templatedUri.With("postId", "456");
            
            var resolvedUri = appendedUri.ToString();

            Assert.AreEqual("https://example.com/users/123/posts/456", resolvedUri);

            // The used parameters should be removed as they are 'consumed'
            Assert.That(parameters, Is.Empty);
        }

        [Test]
        public void KeepsUnresolvedPartsIfNoParameterProvided()
        {
            var parameters = new QueryParameters();
            parameters.Set("userId", "123"); // postId is missing

            var templatedUri = new TemplatedUri("https://example.com/users/{userId}/posts/{postId}", parameters);
            var resolvedUri = templatedUri.ToString();

            Assert.AreEqual("https://example.com/users/123/posts/{postId}", resolvedUri);
        }

        [Test]
        public void ConversionToUriReturnsResolvedUri()
        {
            var parameters = new QueryParameters();
            parameters.Set("userId", "123");
            parameters.Set("postId", "456");

            TemplatedUri templatedUri = new TemplatedUri(
                "https://example.com/users/{userId}/posts/{postId}", 
                parameters
            );
            Uri uri = templatedUri; // Implicit conversion to Uri

            Assert.AreEqual(new Uri("https://example.com/users/123/posts/456"), uri);
        }

        [Test]
        public void ConversionToStringReturnsResolvedUriString()
        {
            var parameters = new QueryParameters();
            parameters.Set("userId", "123");
            parameters.Set("postId", "456");

            TemplatedUri templatedUri = new TemplatedUri(
                "https://example.com/users/{userId}/posts/{postId}", 
                parameters
            );
            string uriString = templatedUri; // Implicit conversion to string

            Assert.AreEqual("https://example.com/users/123/posts/456", uriString);
        }

        [Test]
        public void ConstructorWithUriObjectInitializesCorrectly()
        {
            var baseUri = new Uri("https://example.com/users/{userId}/posts/{postId}");
            var templatedUri = new TemplatedUri(baseUri);

            Assert.AreEqual(baseUri.ToString(), templatedUri.ToString());
        }

        [Test]
        public void GettingUriPartsWithNoPlaceholdersReturnsEmptyList()
        {
            var templatedUri = new TemplatedUri("https://example.com/static/path/no/parameters");
            var parts = templatedUri.GetUriTemplateParts();

            Assert.IsEmpty(parts);
        }
    }
}