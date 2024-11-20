### Using Uxios' Coroutine Support

While Uxios is built around modern asynchronous patterns using promises, there are scenarios where third-party libraries
or other parts of your codebase rely on Unity’s **Coroutines**. Uxios provides utility methods to bridge these two
paradigms, allowing promises to work seamlessly in coroutine-based workflows.

---

### When Should You Use Coroutine support?

1. **Integration with Third-Party Libraries**  
   Some libraries expect data processing or actions to occur within a coroutine. For instance, a library managing
   animations or state changes might only function correctly when chained operations happen in a coroutine flow.

2. **PlayMode Testing in Unity**  
   When writing PlayMode tests, Unity’s test runner moves through the execution flow in a way that doesn’t naturally
   align with promises. Wrapping promises inside coroutines ensures you can control execution timing and perform
   assertions at the right moments.

3. **Blocking Logic in Coroutines**  
   Although not recommended for general usage, there may be cases where you need to block execution within a coroutine
   until an asynchronous request completes (e.g., when synchronizing multiple actions).

---

### How to Use Coroutine Support

#### 1. **Wrap Promises for Coroutine Execution**

Uxios provides the `AsCoroutine` method to wrap a promise, making it compatible with Unity’s coroutine system. This
allows you to start the promise as part of a coroutine, either directly or nested within another coroutine.

**Example:**

```csharp
IEnumerator FetchDataWithCoroutine()
{
    var promise = Uxios.DefaultInstance.Get<string>(new Uri("https://example.com/data"));
    
    // Handle success and error as usual
    promise.Then(OnSuccess);
    promise.Catch(OnError);

    // Wrap the promise in a coroutine to be tracked or chained
    yield return Uxios.AsCoroutine(promise);
}
```

#### 2. **Wait for Promises Inside Coroutines**

If you need to block execution within a coroutine until a promise completes, use the `WaitForRequest` method. This
converts a promise into a `CustomYieldInstruction`, which Unity can process in a `yield return` statement.

**Example:**

```csharp
IEnumerator WaitForRequestExample()
{
    var promise = Uxios.DefaultInstance.Get<string>(new Uri("https://example.com/data"));
    
    yield return Uxios.WaitForRequest(promise);
    
    Debug.Log("This is executed after the request completed - or failed");
}
```

---

### Best Practices

- **Avoid Overusing Blocking**: Blocking coroutines with `WaitForRequest` is not ideal for performance. Promises are
  designed to handle their own lifecycle without needing manual waits.
- **Leverage Promises When Possible**: Use `Then`, `Catch`, and `Finally` methods for asynchronous workflows to maintain
  cleaner, non-blocking code.
- **Understand When Coroutines are Necessary**: Only use coroutines when required by third-party libraries, PlayMode
  tests, or tightly coupled Unity systems.

---

### Key Benefits

- **Flexibility**: Ensures Uxios fits seamlessly into coroutine-heavy workflows or legacy systems.
- **Cross-Paradigm Support**: Allows developers to use promises where supported while maintaining compatibility with
  coroutines.
- **Utility in Testing**: Makes PlayMode tests and coroutine-based assertions easier to manage.

By bridging promises with coroutines, Uxios ensures that you can integrate modern asynchronous workflows into any Unity
project, regardless of its dependency on coroutine-based logic.