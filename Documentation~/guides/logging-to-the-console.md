# Logging Requests, Responses, and Errors

In this guide, we’ll explore how to log HTTP requests, responses, and errors directly to the console in Unity using
Uxios's `ConsoleLogger`. The `ConsoleLogger` auto-registers a series of interceptors that capture and log each request,
response, and any errors that occur. This is particularly useful for debugging and monitoring API interactions in
real-time.

To configure the `ConsoleLogger` to be always active, you can set it up within a `MonoBehaviour` that initializes the
logger when the application starts. By doing this, the `ConsoleLogger` will remain active throughout the app's runtime,
automatically logging each request, response, and any errors to the console. You’ll also need to manage the logger’s
lifecycle to ensure it’s disposed of properly when no longer needed.

Here’s how to set up an always-active `ConsoleLogger` in a `MonoBehaviour`.

---

## Step 1: Create the Logger Manager Script

Create a new script called `GlobalLogger` to manage the `ConsoleLogger` and ensure it remains active for the entire
session.

### GlobalLogger.cs

```csharp
using UnityEngine;
using KindMen.Uxios.Logging;

public class GlobalLogger : MonoBehaviour
{
    private ConsoleLogger logger;

    private void Awake()
    {
        // Ensure only one instance of GlobalLogger exists
        if (FindObjectsOfType<GlobalLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Persist the logger across scenes
        DontDestroyOnLoad(gameObject);

        // Initialize the ConsoleLogger, which auto-registers interceptors
        logger = new ConsoleLogger();
        Debug.Log("ConsoleLogger activated and interceptors registered.");
    }

    private void OnDestroy()
    {
        // Dispose of the logger when this object is destroyed
        logger?.Dispose();
        Debug.Log("ConsoleLogger disposed and interceptors unregistered.");
    }
}
```

### Explanation

1. **Singleton Pattern**:  
   `FindObjectsOfType<GlobalLogger>().Length > 1` ensures that only one instance of `GlobalLogger` exists in the scene.
   If another instance is detected, it’s destroyed to prevent duplicate logging.

2. **Persist Across Scenes**:  
   `DontDestroyOnLoad(gameObject);` keeps the `GlobalLogger` active across scene loads, allowing it to continue logging
   in every scene.

3. **Initialize the ConsoleLogger**:  
   The `ConsoleLogger` is instantiated in `Awake()`, which automatically registers interceptors to log every request,
   response, and error. This way, the logger is configured once and remains active throughout the app’s runtime.

4. **Dispose of the Logger**:  
   `OnDestroy()` ensures that the logger is disposed of properly when the `GlobalLogger` is destroyed, unregistering the
   interceptors and freeing up resources.

---

## Step 2: Add the Logger to Your Scene

1. **Attach `GlobalLogger` to a GameObject**: In the Unity editor, create an empty GameObject in your starting scene (or
   add it to an existing GameObject) and attach the `GlobalLogger` script to it.

2. **Set Up the Uxios Requests**: Since the `ConsoleLogger` is now globally active, you don’t need to set up additional
   logging within your request code. Each request made with Uxios will automatically log details to the console.

---

## Example Usage

Here’s an example of a Uxios request in another script. The `ConsoleLogger` will automatically intercept and log this
request, response, or any errors without additional setup in this script.

```csharp
using System;
using KindMen.Uxios;
using UnityEngine;

public class ExampleRequest : MonoBehaviour
{
    private void Start()
    {
        var url = new Uri("https://httpbin.org/html");
        var config = new Config { TypeOfResponseType = ExpectedTypeOfResponse.Text() };

        // Sending a GET request, which will be logged by the always-active ConsoleLogger
        var uxios = new Uxios();
        uxios.Get(url, config)
            .Then(response => Debug.Log("Received Response: " + response.Data))
            .Catch(error => Debug.LogError("Request failed: " + error.Message));
    }
}
```

## Console Output

With the `ConsoleLogger` configured globally, you’ll see output for each request, response, and error as they occur.

As an example, this is the output for a request:

```plaintext
[UXIOS] Sending request to https://httpbin.org/html: 

{
  "Url": "https://httpbin.org/html",
  "Method": {
    "Method": "GET"
  },
  "BaseUrl": null,
  "TransformRequest": [],
  "TransformResponse": [],
  "Headers": {},
  "Params": [],
  "Data": null,
  "Timeout": 0,
  "Auth": null,
  "TypeOfResponseType": {
    "$type": "KindMen.Uxios.ExpectedTypesOfResponse.TextResponse, com.kind-men.uxios"
  },
  "MaxRedirects": 5
}
```

The `ConsoleLogger` automatically logs:

- **Request Details**: Logs HTTP method, URL, headers, and payload (if applicable).
- **Response Details**: Logs status code and response data.
- **Error Details**: Logs any error messages, including status codes and JSON-formatted error data.

---

## Summary

By setting up the `ConsoleLogger` within a `GlobalLogger` `MonoBehaviour`, you enable always-on logging for Uxios
requests, responses, and errors across your Unity application. This approach simplifies logging, requires minimal setup
in individual scripts, and ensures interceptors are properly registered and disposed of when needed.