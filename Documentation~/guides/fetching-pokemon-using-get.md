# Using `Uxios.Get` to fetch Pokémon

This guide will walk you through using Uxios’s `Get` method to retrieve a typed Pokémon object from
the [Pokémon API](https://pokeapi.co/). This method allows flexibility by letting you add custom configurations to each
request. We’ll also compare this approach with Uxios’s `Resource` wrapper, which is more convenient but less
customizable.

---

## Why Use Uxios’s `Get` Method?

Using Uxios’s `Get` method offers greater **flexibility** because:

1. You can specify a custom configuration object for each request, allowing for fine-tuned control.
2. The `Get` method returns a detailed `Response` object, which includes additional information about the request and
   response.

In contrast, the **`Resource` wrapper** provides a more **convenient** way to work with an API resource but with fewer
configuration options. It also automatically caches responses, making it ideal for situations where you frequently
request the same data.

---

## Step 1: Define a Pokémon Class

Before making the request, define a `Pokemon` class that matches the JSON structure of the API response. This lets Uxios
deserialize the response directly into this object type, making it easier to work with the data in Unity.

```csharp
public class Pokemon
{
    public int id { get; set; }
    public string name { get; set; }
    public int height { get; set; }
    public int weight { get; set; }
}
```

This `Pokemon` class includes the most common fields in a Pokémon API response, which you can expand as needed.

## Step 2: Use Uxios to Fetch Pokémon Data with a Custom Configuration

Now that you have the `Pokemon` class, you can use Uxios’s `Get` method to retrieve data and specify custom
configurations like headers or timeouts.

### Example Code

```csharp
using KindMen.Uxios;
using UnityEngine;
using System;
using System.Collections.Generic;

public class PokemonExample : MonoBehaviour
{
    private void Start()
    {
        var uxios = new Uxios();
        var url = new Uri("https://pokeapi.co/api/v2/pokemon/pikachu");

        // Custom configuration for this request
        var config = new Config
        {
            Headers = new Headers
            {
                { "Authorization", "Bearer your_token_here" }
            },
            Timeout = 5000 // Set a timeout of 5 seconds
        };

        // Perform GET request with custom config and specify Pokemon as the response type
        uxios.Get<Pokemon>(url, config)
            .Then(response =>
            {
                Pokemon pokemon = response.Data;
                Debug.Log("Pokémon Name: " + pokemon.name);
                Debug.Log("Pokémon ID: " + pokemon.id);
                Debug.Log("Height: " + pokemon.height);
                Debug.Log("Weight: " + pokemon.weight);
            })
            .Catch(error =>
            {
                Debug.LogError("Failed to retrieve Pokémon data: " + error.Message);
            });
    }
}
```

### Explanation of the Code

**Custom Configuration (`Config`)**

The `Config` object allows you to specify additional settings for this request:

- **Headers**: Add an authorization header or any other headers you may need.
- **Timeout**: Set a timeout specific to this request (in milliseconds).

**Sending a Typed GET Request**

By specifying `<Pokemon>` as the type parameter for `Get`, Uxios automatically deserializes the response into
a `Pokemon` object, making it easy to work with the data.

**Handling the Response and Errors**

- **`.Then`**: If the request succeeds, this block is executed, and the Pokémon data is available as a `Pokemon`
  object.
- **`.Catch`**: If there’s an error (e.g., network failure), the `.Catch` block handles it.

### Example Output

If the request succeeds, the console might show:

```
Pokémon Name: pikachu
Pokémon ID: 25
Height: 4
Weight: 60
```

If the request fails, you’ll see an error message like:

```
Failed to retrieve Pokémon data: Network error
```

---

## Comparing Uxios’s `Get` Method with the `Resource` Wrapper

### When to Use the `Get` Method

The `Get` method is ideal when you need:

- **Custom configuration** for a specific request (e.g., headers, query parameters, timeouts).
- Access to a detailed `Response` object, which provides extra information, such as status codes and headers.

This flexibility makes it useful for complex or unique API calls where you need more control over each request.

### When to Use the `Resource` Wrapper

The `Resource` wrapper is more convenient for straightforward use cases, where:

- **Automatic caching** is desired, as it caches the response and reuses it until explicitly refreshed.
- You want to **simplify your code** with easy-to-use methods like `Value`, `Update`, and `RemoveAsync` without
  configuring each request individually.

The `Resource` wrapper abstracts away many HTTP details, making it perfect for frequently accessed resources that don’t
need customization for each request.

---

## Summary

Using Uxios’s `Get` method with a typed object provides flexibility and control, allowing you to customize each request
and access a detailed response. The `Resource` wrapper, on the other hand, offers a convenient, simplified interface
with built-in caching.

### Key Takeaways

- **`Get` Method**: Flexible and customizable; ideal for unique or complex requests.
- **`Resource` Wrapper**: Convenient with automatic caching; ideal for frequently accessed resources that don’t require
  extensive configuration.

Choose the approach that best fits your needs, whether you’re looking for flexibility or ease of use in your Unity
project.