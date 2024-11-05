# Fetching and mapping untyped JSON responses

The `Get` method in Uxios is used to make HTTP GET requests, which are commonly used to retrieve data from a server.

This guide will show you how to:

1. Set up and perform a GET request.
2. Handle the response when the request succeeds.
3. Catch and handle errors if the request fails.

In this example, we’ll fetch a sample JSON object from a mock API (`https://jsonplaceholder.typicode.com/todos/1`), a
free API that returns sample data.

## Code Example

Here's the code to make a GET request with Uxios and handle the response or any errors:

```csharp
using KindMen.Uxios;
using UnityEngine;
using System;

public class ExampleGet : MonoBehaviour
{
    private void Start()
    {
        // Initialize Uxios instance
        var uxios = new Uxios();

        // Define the URL to fetch data from
        var url = new Uri("https://jsonplaceholder.typicode.com/todos/1");

        // Perform GET request
        uxios.Get(url)
            .Then(response =>
            {
                // Handle the response on success
                Debug.Log("Status: " + response.Status); // Logs the HTTP status code
                Debug.Log("Content: " + response.Data); // Logs the response content
            })
            .Catch(error =>
            {
                // Handle errors
                Debug.LogError("Request failed: " + error.Message);
            });
    }
}
```
### Explanation of the Code

1. **Initialize Uxios**

   ```csharp
   var uxios = new Uxios();
   ```

   This line creates a new instance of `Uxios`, which provides access to the `Get` method and other HTTP request 
   methods.

2. **Define the URL**

   ```csharp
   var url = new Uri("https://jsonplaceholder.typicode.com/todos/1");
   ```

   Here, we define the URL we want to fetch. In this case, we’re accessing a mock API that returns a sample JSON object 
   with details about a “to-do” item.

3. **Perform the GET Request**

   ```csharp
   uxios.Get(url)
   ```

   The `Get` method sends a GET request to the specified URL. Since this operation takes time (especially with remote 
   servers), `Get` returns a **[promise](../explanation/promises.md)** that we can use to handle the result once it 
   completes.

4. **Handle the Response with `.Then`**

    ```csharp
    .Then(response =>
    {
        Debug.Log("Status: " + response.Status);
        Debug.Log("Content: " + response.Data);
    })
    ```

    - The `.Then` method specifies what to do if the GET request is successful.
    - `response.Status`: Logs the HTTP status code (e.g., `200` for OK) to help understand the server’s response.
    - `response.Data`: Logs the content of the response, which contains the actual data returned by the server.

5. **Handle Errors with `.Catch`**

   ```csharp
   .Catch(error =>
   {
       Debug.LogError("Request failed: " + error.Message);
   });
   ```

   - The `.Catch` method specifies what to do if the GET request fails.
   - `error.Message`: Logs the error message, providing details about why the request failed (e.g., network issues or 
     server errors).

## Understanding the Response Object

The response object returned by `Uxios.Get` contains important data, such as:

- **Status**: The HTTP status code (e.g., `200` for a successful response).
- **Data**: The content of the response. This is where your actual data resides.

In this example, `response.Data` holds a Newtonsoft JObject instance representing the JSON response, which you can 
query or use directly in your application.

## Summary

- **`uxios.Get(url)`**: Sends a GET request to the specified URL.
- **`.Then(response => { ... })`**: Handles the successful response, logging the status code and response data.
- **`.Catch(error => { ... })`**: Catches any errors, logging a failure message.

This setup makes it easy to manage API calls, allowing your game to retrieve data from remote servers without freezing 
or disrupting gameplay. You can also chain additional `.Then` or `.Catch` handlers if you need more complex workflows.