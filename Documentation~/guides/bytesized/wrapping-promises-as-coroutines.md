# Wrapping Promises as Coroutines

## Overview

Sometimes, you need to execute a promise inside Unity’s coroutine system — for example, when a library or system 
expects a coroutine. Use `AsCoroutine` to make any promise compatible with Unity coroutines.

## Steps

```csharp
IEnumerator FetchData()
{
    // 1. Create the Promise
    var promise = Uxios.DefaultInstance.Get<string>(new Uri("https://example.com/data"));

    promise.Then(OnSuccess);
    promise.Catch(OnError);

    // 2. Wrap the Promise as IEnumerator
    yield return Uxios.AsCoroutine(promise);
}

void OnSuccess(string data)
{
    Debug.Log("Data received: " + data);
}

void OnError(Exception e)
{
    Debug.LogError("Request failed: " + e.Message);
}
```

### Quick Notes

- **Use `AsCoroutine`** for seamless promise execution in coroutines.
