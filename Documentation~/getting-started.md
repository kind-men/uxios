# Getting Started

Welcome to the Uxios documentation! Uxios is a Unity-friendly library inspired by Axios, designed to make HTTP
interactions in Unity simple, efficient, and familiar. In this guide, we’ll introduce the `Resource` class as an easy
way to get started with basic HTTP interactions. We’ll also show you how to use Uxios’s core HTTP methods (like `Get`)
directly for more control when you need it.

---

## Overview of the `Resource` Class

The `Resource` class in Uxios provides an easy-to-use interface for working with individual API resources. It’s ideal if
you need basic CRUD (Create, Read, Update, Delete) operations without worrying about low-level HTTP configuration.
Using `Resource`, you can:

1. **Retrieve a Resource**: Automatically fetch and cache data for a resource with lazy loading.
2. **Check if a Resource Exists**: Easily check if a resource exists on the server.
3. **Update or Delete a Resource**: Modify or remove a resource as needed.

The `Resource` class manages caching and basic error handling, making it an ideal starting point for common use cases.

---

## Using the `Resource` Class

To use `Resource`, you simply define the endpoint URI and specify a type for the data you expect to receive. Let’s walk
through how to create a `Resource` instance, check if it exists, fetch its data, update it, and delete it.

### Step 1: Define a Resource

First, define the URI of the resource and specify a type. Here’s how you can set up a `Resource` for a "to-do" item in a
REST API (like the JSONPlaceholder API):

```csharp
using KindMen.Uxios.Api;
using UnityEngine;
using System;

public class Example : MonoBehaviour
{
    private void Start()
    {
        // Define the base URI for the to-do item
        Uri todoUri = new Uri("https://jsonplaceholder.typicode.com/todos/1");

        // Create a Resource instance
        var todoResource = new Resource<Todo>(todoUri);
    }
}
```

In this example:

- `Resource<Todo>` is created with `todoUri`, where `Todo` is a class that matches the expected structure of the JSON
  data.

### Step 2: Check if the Resource Exists

The `HasValue` property allows you to check if a resource exists before attempting to fetch or use it. This is
especially useful for cases where the resource might not be available, and you want to handle that gracefully.

```csharp
todoResource.HasValue
    .Then(exists =>
    {
        if (exists)
        {
            Debug.Log("Resource exists!");
        }
        else
        {
            Debug.Log("Resource does not exist.");
        }
    })
    .Catch(error =>
    {
        Debug.LogError("Error checking resource existence: " + error.Message);
    });
```

### Step 3: Retrieve the Resource Data

Once you know the resource exists, you can retrieve its data using the `Value` property. This property fetches the data
if it isn’t already cached, providing lazy loading with built-in caching.

```csharp
todoResource.Value
    .Then(todo =>
    {
        Debug.Log("Title: " + todo.Title);
        Debug.Log("Completed: " + todo.Completed);
    })
    .Catch(error =>
    {
        Debug.LogError("Error fetching resource data: " + error.Message);
    });
```

In this example:

- `todoResource.Value` returns a `Promise<Todo>`, where `Todo` is your data model. When resolved, you can access the
  fields directly.

### Step 4: Update the Resource

The `Update` method allows you to modify a resource and send the updated data to the server.

```csharp
var updatedTodo = new Todo { Title = "Updated Title", Completed = true };

todoResource.Update(updatedTodo)
    .Then(todo =>
    {
        Debug.Log("Updated Title: " + todo.Title);
    })
    .Catch(error =>
    {
        Debug.LogError("Error updating resource: " + error.Message);
    });
```

### Step 5: Delete the Resource

The `Remove` method allows you to delete the resource from the server and clears the cached data.

```csharp
todoResource.Remove()
    .Then(() => Debug.Log("Resource deleted."))
    .Catch(error => Debug.LogError("Error deleting resource: " + error.Message));
```

---

## Using Uxios Directly for More Control

While the `Resource` class is great for straightforward use cases, Uxios also provides direct HTTP
methods (`Get`, `Post`, `Put`, `Delete`, etc.) for more advanced control. Using these methods directly offers several
advantages:

1. **Custom Configuration**: Uxios’s HTTP methods accept configuration options, allowing you to set headers, timeouts,
   query parameters, and more.
2. **Detailed Response Object**: When using `Get` directly, you receive a `Response` object, which includes detailed
   information about the request, such as the status code, headers, and the raw data.

Here’s an example of using `Uxios.Get` with a custom configuration:

### Example: Fetching Data with `Uxios.Get`

```csharp
using KindMen.Uxios;
using UnityEngine;
using System;

public class ExampleGet : MonoBehaviour
{
    private void Start()
    {
        var uxios = new Uxios();
        var url = new Uri("https://jsonplaceholder.typicode.com/todos/1");

        // Perform GET request with custom configuration
        uxios.Get<Todo>(url, new Config
        {
            Headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer your_token_here" }
            },
            Timeout = 5000 // Set timeout to 5 seconds
        })
        .Then(response =>
        {
            Debug.Log("Status: " + response.Status);
            Debug.Log("Title: " + response.Data.Title);
        })
        .Catch(error =>
        {
            Debug.LogError("Request failed: " + error.Message);
        });
    }
}
```

In this example:

- `Uxios.Get<Todo>` performs a GET request to the specified URL.
- A custom `Config` is used to add an `Authorization` header and set a 5-second timeout.
- The `Response` object provides access to the status code (`response.Status`) and the typed data (`response.Data`).

### Advantages of Direct Uxios Methods

- **Full HTTP Control**: Customize requests for specific needs, such as setting headers, timeouts, or query parameters.
- **Detailed Response Information**: The `Response` object contains additional information (like headers and status
  code) that isn’t always needed but can be useful for debugging or special cases.
- **Better for Complex API Calls**: Direct methods are ideal for more complex or custom API interactions, where the
  simpler `Resource` abstraction may be limiting.

---

## Summary

The `Resource` class in Uxios is a simple and powerful way to start working with API resources in Unity, providing an
easy interface for basic CRUD operations and caching. For more control over HTTP requests, Uxios’s core
methods (`Get`, `Post`, `Put`, `Delete`) give you advanced configuration options and additional response details.

### When to Use `Resource` vs. Direct Uxios Methods

- **Use `Resource`**: For straightforward CRUD operations with basic caching and easy access to resource existence
  checks.
- **Use Direct Uxios Methods**: For complex API interactions where you need fine-tuned control, custom headers, query
  parameters, or more response details.

This flexible setup lets you choose the approach that best fits your needs, whether you’re looking for simplicity or
advanced control in your Unity project.
