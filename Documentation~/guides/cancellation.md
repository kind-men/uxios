# Cancelling Requests

In some scenarios, you may want to cancel a long-running request if it’s no longer needed, such as when a user navigates away from a page or dismisses a loading UI. Uxios provides built-in support for request cancellation, making it easy to manage these situations effectively.

## Introduction

This guide explains how to cancel requests using Uxios’s support for `CancellationTokenSource`. By attaching a cancellation token to the request configuration, you can interrupt a request in progress, which stops the network call and returns a `ConnectionError` to signify that the request was intentionally aborted.

### Example Use Cases

- Cancelling a data fetch if the user leaves a page or closes a dialog.
- Stopping requests that are no longer relevant, saving resources and improving performance.

---

## Step-by-Step Instructions

### Step 1: Create a Cancellation Token

To cancel a request, start by creating a `CancellationTokenSource`. This token will be attached to the request and allows you to cancel it as needed.

```csharp
var cancellationSource = new CancellationTokenSource();
```

### Step 2: Attach the Cancellation Token to the Request Configuration

Next, configure the request with the cancellation token. Use the `CancelUsing` method on the `Config` object to attach the token. This configuration will be passed along with the request, enabling cancellation.

```csharp
var customConfig = Config.Default();
var configWithCancellation = customConfig.CancelUsing(cancellationSource);
```

### Step 3: Start the Request

Initiate the request using Uxios, passing in the configured `Config` object. The request will listen for cancellation instructions.

```csharp
var url = new Uri("https://httpbin.org/delay/10");
var promise = uxios.Get<string>(url, configWithCancellation);
```

In this example:

- The URL `https://httpbin.org/delay/10` simulates a delay, allowing you to test the cancellation mechanism.
- The `promise` returned by `Get` will be used to track and handle the request’s completion or cancellation.

### Step 4: Cancel the Request

After starting the request, you can cancel it by calling `Cancel` on the `CancellationTokenSource`. For instance, if a user action or other condition triggers the cancellation, simply execute:

```csharp
cancellationSource.Cancel();
```

This aborts the request as soon as possible and returns a `ConnectionError` with a message indicating that the request was aborted.

---

## Example Code

Here’s the full code example demonstrating how to initiate and cancel a request in Uxios:

```csharp
using System;
using System.Threading;
using KindMen.Uxios;
using UnityEngine;
using UnityEngine.TestTools;

public class Example : MonoBehaviour
{
    private Uxios uxios = new Uxios();

    public IEnumerator CancelARequest()
    {
        var url = new Uri("https://httpbin.org/delay/10");

        // Step 1: Create a Cancellation Token
        var cancellationSource = new CancellationTokenSource();
        
        // Step 2: Attach the Cancellation Token to the Config
        var customConfig = Config.Default();
        var configWithCancellation = customConfig.CancelUsing(cancellationSource);
        
        // Step 3: Start the Request
        var promise = uxios.Get<string>(url, configWithCancellation);

        // Simulate a delay before cancelling
        yield return new WaitForSeconds(1);
        
        // Step 4: Cancel the Request
        cancellationSource.Cancel();
    }
}
```

---

## Summary

In this guide, you learned how to cancel a request in Uxios using a `CancellationTokenSource`. By attaching a cancellation token to the request configuration, you can manage long-running or unnecessary requests and respond to user interactions effectively.

- **Step 1**: Create a `CancellationTokenSource`.
- **Step 2**: Attach the token to the request configuration.
- **Step 3**: Start the request.
- **Step 4**: Cancel the request when needed.

This approach allows for efficient request management, helping you optimize performance and resource use in your Unity projects.