# Building an Automatic Loader

In this guide, we’ll create a loader panel in Unity that automatically shows when a request is in progress and hides
when the request completes. The example uses a `PromiseLoaderPanel` class with the `ShowWhile` method to manage the
loader's visibility. This loader works with promises from the **RSG Promises** library, ensuring that the loader panel
remains active until the promise resolves (successfully or with an error) and then automatically hides.

This approach keeps your code clean and provides a smooth user experience, as the loader is only visible when needed.

---

## How It Works

The `PromiseLoaderPanel` class is a Unity component that displays a UI panel when a request is running. By calling
the `ShowWhile` method, you:

1. Activate the loader panel when the request starts.
2. Automatically hide the loader panel when the request completes, whether it succeeds or fails.

Using the `.Finally` method on a promise ensures that the loader panel will always hide, regardless of whether the
promise resolves successfully or with an error.

---

## Step 1: Set Up the Loader Panel in Unity

1. **Create a New UI Panel**:

    - In the Unity editor, go to **GameObject > UI > Panel** to create a new UI panel.
    - Name it `LoaderPanel` and customize its appearance as desired (e.g., add a loading spinner or "Loading..." text).

2. **Add the `PromiseLoaderPanel` Script**:

    - Create a new C# script called `PromiseLoaderPanel`.
    - Attach this script to the `LoaderPanel` GameObject.

3. **Set the Panel to Inactive**:

    - In the Unity editor, select the `LoaderPanel` and uncheck the **Active** checkbox to ensure it starts inactive.

---

## Step 2: Write the `PromiseLoaderPanel` Script

Here’s the code for `PromiseLoaderPanel`, which includes the `ShowWhile` method:

```csharp
using RSG;
using UnityEngine;

namespace KindMen.Uxios.UI
{
    public class PromiseLoaderPanel : MonoBehaviour
    {
        /// <summary>
        /// Displays the panel while the promise is pending and hides it after the promise resolves,
        /// regardless of success or failure.
        /// </summary>
        /// <param name="promise">The promise to track for completion.</param>
        /// <typeparam name="TData">The type of data returned by the promise.</typeparam>
        /// <returns>The original promise, allowing further chaining.</returns>
        public IPromise<TData> ShowWhile<TData>(IPromise<TData> promise)
        {
            // Show the loader panel when the request starts
            gameObject.SetActive(true);

            // Hide the loader panel when the request completes, whether successful or failed
            return promise.Finally(() => gameObject.SetActive(false));
        }
    }
}
```

### Explanation of the Code

**`ShowWhile<TData>`**:

- This method takes a promise (`IPromise<TData>`) as a parameter, where `TData` is the data type the promise will
  return upon completion.
- `gameObject.SetActive(true);`: Activates the loader panel when the request begins.
- `.Finally(() => gameObject.SetActive(false));`: The `.Finally` method hides the panel once the promise completes,
  whether it succeeds or fails. This guarantees that the loader panel will always hide, regardless of the request's
  outcome.
- The method returns the original promise, allowing further chaining of `.Then` or `.Catch` handlers if needed.

---

## Step 3: Use the Loader in Your Code

Now that the loader panel is set up, you can use it to automatically manage the visibility of the loader whenever a
promise-based request is made.

### Example

Here’s an example of how to use the loader panel with a request:

```csharp
using KindMen.Uxios;
using KindMen.Uxios.UI;
using UnityEngine;
using System;

public class ExampleLoaderUsage : MonoBehaviour
{
    public PromiseLoaderPanel loaderPanel;

    private void Start()
    {
        // Define the URL for a sample request
        var url = new Uri("https://jsonplaceholder.typicode.com/todos/1");
        var uxios = new Uxios();

        // Make a GET request and show the loader while the request is pending
        loaderPanel.ShowWhile(uxios.Get(url))
            .Then(response =>
            {
                Debug.Log("Request successful: " + response.Data);
            })
            .Catch(error =>
            {
                Debug.LogError("Request failed: " + error.Message);
            });
    }
}
```

In this example:

- `loaderPanel.ShowWhile(uxios.Get(url))` starts a GET request to `url`, showing the loader panel while the request is
  pending.
- The `Then` lambda function logs the response data if the request succeeds.
- The `Catch` lambda function logs an error if the request fails.

Because of the `.Finally` in `ShowWhile`, the loader panel will automatically hide once the request completes,
regardless of the outcome.

---

## Summary

This setup allows you to easily manage a loading UI for asynchronous operations, showing and hiding the loader as
needed. Here’s a quick recap:

1. **Loader Activation**: The `ShowWhile` method activates the loader when a promise is pending.
2. **Automatic Deactivation**: Using `.Finally`, the loader hides itself once the promise completes.
3. **Flexible Use**: You can use `ShowWhile` with any promise-based operation to ensure the loader appears only when
   needed.

This approach keeps your UI responsive and user-friendly, providing feedback to the player while the application is
loading or processing data. You can further customize the loader panel by adding animations, custom messages, or
progress indicators to enhance the experience.