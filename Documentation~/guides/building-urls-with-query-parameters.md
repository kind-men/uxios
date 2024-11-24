# Dynamically Building URIs with Query Strings

## Introduction

In many applications, you might need to dynamically build URIs with query parameters based on user inputs or application
state. Uxios provides tools like the `QueryString` utility and the `Resource` class to streamline this process. This
guide will walk you through an example of combining an object representing query values with a
base `System.Uri` to dynamically construct a URI. We'll then demonstrate how to pass this URI to a `Resource` while also
highlighting the flexibility of the `With` method for additional customizations.

---

## Example Use Cases

1. **Search Functionality**: Dynamically generate a URI for an API search endpoint, where query parameters include
   filters, pagination, and sorting.
2. **Data-Driven Dashboards**: Build URIs based on user-selected criteria, such as date ranges or selected metrics.
3. **Dynamic API Requests**: Construct endpoints that adapt based on runtime data, such as user-specific or
   device-specific parameters.

---

## Step-by-Step Instructions

### Step 1: Define the Query Parameter Object

Define an object that represents the values for the query string. This could be a simple class or an anonymous object.

```csharp
var queryParameters = new
{
    search = "Unity",
    page = 1,
    limit = 10,
    sort = "desc"
};
```

---

### Step 2: Serialize the Query Object into a Query String

Use the `QueryString.Serialize` method to convert the object into a URL-encoded query string.

```csharp
string queryString = QueryString.Serialize(queryParameters);
// Output: "search=Unity&page=1&limit=10&sort=desc"
```

---

### Step 3: Combine the Query String with a Base URI

Combine the query string with a base URI to dynamically construct the full URI.

```csharp
Uri baseUri = new Uri("https://api.example.com/resources");
string fullUri = $"{baseUri}?{queryString}";
// Output: "https://api.example.com/resources?search=Unity&page=1&limit=10&sort=desc"
```

Alternatively, you can create a `System.UriBuilder` for better control:

```csharp
var uriBuilder = new UriBuilder(baseUri)
{
    Query = queryString
};

Uri finalUri = uriBuilder.Uri;
// Output: "https://api.example.com/resources?search=Unity&page=1&limit=10&sort=desc"
```

---

### Step 4: Pass the URI to the Uxios Resource

Create a `Resource` instance with the dynamically built URI and use it to make API calls.

```csharp
var resource = new Resource(finalUri);
resource.Get().Then(response =>
{
    Debug.Log($"Data received: {response.Data}");
}).Catch(error =>
{
    Debug.LogError($"Error: {error.Message}");
});
```

---

### Alternative: Customize Query Parameters Using the `With` Method

The `Resource` class also allows adding or overriding query parameters at runtime using the `With` method. This is
useful for cases where query parameters are partially predefined but need dynamic adjustments.

```csharp
resource.With(new QueryParameters
{
    search = "C#",
    page = 2
}).Get().Then(response =>
{
    Debug.Log($"Data received for page 2: {response.Data}");
}).Catch(error =>
{
    Debug.LogError($"Error: {error.Message}");
});
```

---

## Full Code Example

```csharp
using System;
using KindMen.Uxios;
using UnityEngine;

public class QueryStringExample : MonoBehaviour
{
    void Start()
    {
        // Step 1: Define the query parameters
        var queryParameters = new
        {
            search = "Unity",
            page = 1,
            limit = 10,
            sort = "desc"
        };

        // Step 2: Serialize the object into a query string
        string queryString = QueryString.Serialize(queryParameters);

        // Step 3: Combine with a base URI
        Uri baseUri = new Uri("https://api.example.com/resources");
        var uriBuilder = new UriBuilder(baseUri)
        {
            Query = queryString
        };

        Uri finalUri = uriBuilder.Uri;

        // Step 4: Pass the URI to the Resource
        var resource = new Resource(finalUri);
        resource.Get().Then(response =>
        {
            Debug.Log($"Data received: {response.Data}");
        }).Catch(error =>
        {
            Debug.LogError($"Error: {error.Message}");
        });

        // Alternative: Pass query parameters using `With`
        resource.With(new QueryParameters
        {
            search = "C#",
            page = 2
        }).Get().Then(response =>
        {
            Debug.Log($"Data received for page 2: {response.Data}");
        }).Catch(error =>
        {
            Debug.LogError($"Error: {error.Message}");
        });
    }
}
```

---

## Summary

This guide demonstrates how to:

1. Define an object representing query parameters.
2. Convert it to a query string using `QueryString.Serialize`.
3. Dynamically build a URI with `System.Uri` or `UriBuilder`.
4. Pass the URI to a `Resource` for API requests.
5. Use the `With` method for additional flexibility.

The `QueryString` utility and `Resource` class in Uxios simplify handling query strings, making your Unity applications
more dynamic and adaptable.