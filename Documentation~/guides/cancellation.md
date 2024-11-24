# Cancelling Requests

In some scenarios, you may want to cancel a long-running HTTP request if it’s no longer needed, such as when a user
navigates away from a page or dismisses a loading UI. Uxios provides built-in support for cancelling requests,
simplifying how you manage these situations.

This guide explains how to cancel requests in Uxios using the `Abort` method or through coroutine-based automatic
cancellation. By attaching promises to Uxios’s `AbortController`, you can efficiently manage the lifecycle of HTTP
requests in your Unity projects.

!!! info "Cancellation Mechanism"

    It’s the **HTTP request** that gets cancelled, not the promise itself. If the HTTP request has already completed,
    cancellation has no effect on the handling of the response.

## Example Use Cases

- Cancelling a data fetch if the user leaves a page or closes a dialog.
- Stopping requests that are no longer relevant, saving resources and improving performance.
- Automatically aborting requests associated with coroutines when they stop.

---

## Step 1: Start the Request

Initiate the request using Uxios as you normally would. A promise is returned, which you can use to track and handle the
request.

```csharp
var url = new Uri("https://httpbin.org/delay/10");
var promise = uxios.Get<string>(url);
```

In this example:

- The URL `https://httpbin.org/delay/10` simulates a delay, allowing you to test the cancellation mechanism.

## Step 2: Cancel the Request

To cancel the request, simply call the `Abort` method on the Uxios instance and pass in the promise. For example, if a
user action triggers cancellation:

```csharp
uxios.Abort(promise);
```

This aborts the associated HTTP request as soon as possible. If the request was already completed, this action will have
no effect on the response handling.

---

## Using Promises in Coroutines

When using [promises in a coroutine](../explanation/coroutines.md), you can utilize Uxios's built-in coroutine helpers, 
such as `WaitForRequest` or `AsCoroutine`. These methods ensure that:

1. The promise remains alive as long as the coroutine is running.
2. If the coroutine unexpectedly stops, Uxios automatically calls `Abort` on the associated promise.

Example:

```csharp
private IEnumerator FetchData()
{
    var url = new Uri("https://httpbin.org/delay/10");
    var promise = uxios.Get<string>(url);

    yield return promise.WaitForRequest();

    Debug.Log("Request completed: " + promise.CurState);
}

private StoppingACoroutine()
{
    var coroutine = StartCoroutine(FetchData);
    
    // This will automatically abort the request in the
    // next frame
    StopCoroutine(coroutine);
}        
```

If the coroutine is stopped, the associated promise will be aborted automatically. There’s no need to manually manage
cancellation in this scenario.

---

## Summary

In this guide, you learned how to cancel requests in Uxios using the `Abort` method and the automatic cancellation
mechanism for coroutines. Key takeaways:

- **Step 1**: Start a request as usual.
- **Step 2**: Cancel a request manually by calling `Abort` on the promise.
- **Coroutine Handling**: Promises used in coroutines are automatically aborted when the coroutine stops.
