# Understanding Promises in Unity

## What Are Promises?

A Promise is a tool for managing asynchronous code, which means code that can run in the background and complete
sometime in the future. Instead of freezing the game while waiting for an operation (like fetching data from a server),
promises allow the game to keep running smoothly.

In Unity, you’re probably familiar with `coroutines`, which also handle asynchronous actions. Promises are another way 
to handle these tasks, but with a different structure that provides better control, especially when dealing with complex
sequences or dependencies between actions.

## Why Use Promises?

Imagine you’re writing a Unity game that needs to:

1. Check if a player’s profile exists on a remote server.
2. If it exists, fetch the profile data.
3. Then update the profile with new data.

Using promises allows you to define this workflow in a clear, structured way, so each step only begins when the previous
one has finished. Promises also make it easy to handle errors at each step, simplifying complex sequences of actions.

## Key Concepts of Promises

1. **A Promise Represents a Future Result** Think of a promise as a placeholder for a result that hasn’t arrived yet. 
   The promise starts pending (waiting for the result). Once the operation finishes, the promise is either:
   
   * Fulfilled (operation succeeded), or
   * Rejected (operation failed with an error).

2. **Chaining** Promises can be chained together, meaning you can specify that once one promise finishes, another action
   should start. This makes it easier to set up sequences of tasks without nesting coroutines.

3. **Error Handling** Promises let you handle errors at any step in the chain, helping to keep your code cleaner and 
   ensuring that errors don’t stop the entire sequence.

## How Promises Work

A promise is created with an operation, like fetching data or performing a calculation, that runs asynchronously. When
this operation completes, the promise can do one of two things:

- **Resolve (Success)**: The operation succeeded, and the result is ready.
- **Reject (Failure)**: The operation failed, usually due to an error (e.g., network failure).
- 
You can “listen” for these outcomes and specify actions to take in each case.

## Using Promises in Unity

###Creating and Handling Promises

In Unity, the **RSG Promises** library (https://github.com/Real-Serious-Games/C-Sharp-Promise) is commonly used for 
handling promises. 

Here’s a basic example:

```csharp
using RSG;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        FetchDataFromServer()
            .Then(data => Debug.Log($"Data fetched: {data}"))
            .Catch(error => Debug.LogError($"Error fetching data: {error.Message}"));
    }

    private IPromise<string> FetchDataFromServer()
    {
        return new Promise<string>(() =>
        {
            // Simulate an async operation (e.g., network request)
            bool success = true; // Assume the request is successful

            if (success)
            {
                return "Player data here"; // Resolve the promise with the result
            }
            
            throw new System.Exception("Failed to fetch data");
        });
    }
}
```

### Explanation of the Code

1. `FetchDataFromServer`:

    - This method creates a new `Promise<string>`, meaning it will eventually return a `string` value.
    - Inside the promise, you either return (success) data or throw an exception (error) if something goes wrong.

2. Handling the Result with `.Then`:

   - The `Then` method specifies what happens when the promise resolves successfully. In this case, it logs the fetched 
     data.

3. Handling Errors with `.Catch`:

   - The `Catch` method specifies what happens if the promise is rejected. Here, it logs an error message if fetching 
     data fails.

### Why Promises Are Better for Complex Sequences

Suppose you want to perform multiple asynchronous operations in a specific order. With promises, you can **chain** 
actions, ensuring each one completes before the next begins.

For example, if you want to:

1. Check if data exists,
2. Fetch it if it does, and
3. Update it afterward,

you could write this with promises as follows:

```csharp
private void Start()
{
    CheckDataExists()
        .Then(exists =>
        {
            if (exists)
            {
                return FetchData();
            }
            else
            {
                return Promise<string>.Rejected("No data to fetch.");
            }
        })
        .Then(data => UpdateData(data))
        .Catch(error => Debug.LogError($"An error occurred: {error.Message}"));
}

private IPromise<bool> CheckDataExists()
{
    return new Promise<bool>((resolve, reject) =>
    {
        // Simulate a check
        bool exists = true;
        resolve(exists);
    });
}

private IPromise<string> FetchData()
{
    return new Promise<string>((resolve, reject) =>
    {
        // Simulate fetching data
        resolve("Fetched data");
    });
}

private IPromise UpdateData(string data)
{
    return new Promise((resolve, reject) =>
    {
        // Simulate updating data
        Debug.Log($"Data updated: {data}");
        resolve();
    });
}
```

In this example:

- Each method (CheckDataExists, FetchData, UpdateData) returns a promise.
- The promises are chained together using Then, so each step only begins after the previous one finishes.
- If any step fails, the Catch at the end handles the error.

### Summary of Key Methods

- `Then`: Specifies what to do when the promise succeeds. You can use it to chain multiple actions together.
- `Catch`: Specifies what to do if the promise fails, catching errors and preventing them from stopping the entire 
  sequence.
- `All` (**Advanced**): Allows multiple promises to run in parallel and waits for all of them to complete. Useful when 
  fetching multiple pieces of data at the same time.

## When to Use Promises vs. Coroutines

Both promises and coroutines handle asynchronous tasks, but promises provide more control, better error handling, and
chaining for complex sequences. Use **coroutines** for simpler tasks or when you don’t need extensive error handling or
sequencing. Use **promises** when you have multiple asynchronous operations that depend on each other, need precise 
control, or require robust error handling.

## Final Thoughts

Promises give you the power to structure asynchronous workflows with ease, clarity, and control. They’re a bit like
coroutines but designed to manage complex, dependent actions and errors more gracefully. Once you get the hang of
promises, you’ll find them a powerful tool for building responsive and stable Unity applications!