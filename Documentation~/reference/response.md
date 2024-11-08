# `Response` Object

The `Response` class in Uxios represents the result of an HTTP request, containing detailed information about the
server's response. This includes the response data, status code, headers, and additional configuration details.
The `Response` object allows you to easily access both the returned data and metadata about the request, making it a
core component for handling HTTP interactions in Uxios.

## Overview of the `Response` Fields and Methods

The `Response` class provides several fields to capture key aspects of the server’s response. Here’s an in-depth look at
each field and method:

## Fields

### Data

**Type**: `object`

**Description**: Contains the deserialized content returned by the server, such as JSON data. The `Data` field holds the
core information you requested, whether it’s a JSON object, array, or other format.

**Usage**: The `Data` field is typically typed using a generic parameter provided to Uxios methods like `Get`, `Post`,
or `Put`, or when using the `Resource` wrapper. For instance, `Uxios.Get<Pokemon>(url)` will type `Data` as `Pokemon`,
allowing you to interact with it directly as a strongly-typed object.

**Example**:

```csharp
var url = new Uri("https://pokeapi.co/api/v2/pokemon/pikachu");
uxios.Get<Pokemon>(url)
    .Then(response =>
    {
        Pokemon pokemon = response.Data;
        Debug.Log("Pokémon Name: " + pokemon.Name);
    });
```

### Status

**Type**: `HttpStatusCode`

**Description**: Stores the HTTP status code returned by the server, indicating the success or failure of the request.
Common status codes include `200` for success, `404` for not found, and `500` for server error.

**Usage**: The `Status` field is useful for quickly checking the outcome of the request and can also be validated with
custom logic using the `IsValid()` method (described below).

**Example**:

```csharp
if (response.Status == HttpStatusCode.OK)
{
    Debug.Log("Request was successful!");
}
```

### Headers

**Type**: `Headers`

**Description**: Contains HTTP headers as key-value pairs, providing metadata about the response (e.g., "Content-Type"
or "Authorization"). Headers are useful for accessing additional context about the response, such as content encoding,
cache information, or server type.

**Usage**: You can access headers individually to get more information about the response, which can be helpful for
debugging or handling specific response conditions.

**Example**:

```csharp
string contentType = response.Headers["Content-Type"];
Debug.Log("Response Content Type: " + contentType);
```

### Config

**Type**: `Config`

**Description**: Stores the configuration settings used for the request, including options such as timeout settings,
custom headers, and status validation rules.

**Usage**: The `Config` field lets you review the settings used for the request, which is helpful for ensuring that
custom configurations (such as authorization headers or query parameters) were applied as expected.

**Example**:

```csharp
Debug.Log("Request Timeout: " + response.Config.Timeout);
```

### Request

**Type**: `Request`

**Description**: Represents the original request that initiated the response, providing access to details about the
request, such as the URL, HTTP method, and any parameters or body data.

**Usage**: The `Request` field is useful if you need to inspect the original request details, especially in complex
workflows where multiple requests may be involved.

**Example**:

```csharp
Debug.Log("Request URL: " + response.Request.Url);
```

## Methods

### IsValid()

**Type**: `bool`

**Description**: Validates the HTTP status code of the response based on a validation function defined in the `Config`
object. This function determines if the status code falls within the expected range or matches specific criteria for a
successful response.

**Usage**: The `IsValid` method checks if the response status is valid according to custom validation logic, often set
in the request configuration. This is helpful when working with APIs that may use different status code conventions or
when you need to handle specific ranges of status codes.

**Example**:

```csharp
if (response.IsValid())
{
    Debug.Log("Response status is valid.");
}
else
{
    Debug.LogWarning("Response status is not valid.");
}
```

---

## Example Usage

Here's an example demonstrating how to use the `Response` object fields and methods when making a GET request to
retrieve a typed Pokémon object.

```csharp
using KindMen.Uxios;
using UnityEngine;
using System;

public class Example : MonoBehaviour
{
    private void Start()
    {
        var uxios = new Uxios();
        var url = new Uri("https://pokeapi.co/api/v2/pokemon/pikachu");

        uxios.Get<Pokemon>(url)
            .Then(response =>
            {
                // Access the typed data directly
                Pokemon pokemon = response.Data;
                Debug.Log("Pokémon Name: " + pokemon.Name);
                
                // Check the status code
                Debug.Log("Status: " + response.Status);
                
                // Access a specific header
                if (response.Headers.ContainsKey("Content-Type"))
                {
                    Debug.Log("Content-Type: " + response.Headers["Content-Type"]);
                }
                
                // Validate the response status
                if (response.IsValid())
                {
                    Debug.Log("Response is valid.");
                }
                else
                {
                    Debug.LogWarning("Response status is not valid.");
                }
            })
            .Catch(error =>
            {
                Debug.LogError("Request failed: " + error.Message);
            });
    }
}
```

---

## Key Points

- **Data**: Holds the main content of the response, typed as a generic type when provided.
- **Status**: Stores the HTTP status code, helpful for quickly checking success or failure.
- **Headers**: Provides access to response headers for additional metadata.
- **Config**: Contains request settings, allowing review of the request’s configuration.
- **Request**: The original request object, useful for inspecting the request details.

The `Response` object in Uxios provides all the necessary information about an HTTP response in a structured and
accessible way, making it easy to handle both data and metadata about each request in Unity.