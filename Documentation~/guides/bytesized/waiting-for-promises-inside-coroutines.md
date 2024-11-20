# Waiting for Promises Inside Coroutines

## Overview

When you need to block a coroutine until a promise completes, use `WaitForRequest`. This ensures the coroutine only
continues after the promise resolves or rejects. While not ideal for performance, this is useful for tightly integrated
workflows or tests.

## Steps

```csharp
IEnumerator WaitForData()
{
    // 1. Create the Promise
    var promise = Uxios.DefaultInstance.Get<string>(new Uri("https://example.com/data"));

    // 2. Wait for the Promise to resolve
    yield return UxiosHelpers.WaitForRequest(promise);

    // 3. Perform actions after the promise resolved
    Debug.Log("Promise is done, meaning any Then and Catch statements have been executed");
}
```

---

### Quick Notes

- **Use `WaitForRequest`** when blocking is necessary, such as in tests or specific workflows.