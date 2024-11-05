# Using `Resource` to fetch Pokémon

The Pokémon API is a RESTful API that provides data on Pokémon, moves, abilities, and more. In this example, we’ll use
it to retrieve data for a specific Pokémon by its ID or name.

## Step 1: Create a Resource for a Pokémon

First, let's set up a `Resource` instance for a specific Pokémon, such as Pikachu. We’ll use a `System.Uri` to define 
the Pokémon’s URL, and we’ll define a `Pokemon` class to represent the Pokémon data model.

**Example: Setting up the Resource**

```csharp
using System;
using KindMen.Uxios.Api;

Uri url = new Uri("https://pokeapi.co/api/v2/pokemon/pikachu");

// Create a Resource for Pikachu by URL
var pikachu = new Resource<Pokemon>(url);
```

## Step 2: Check if the Pokémon Resource Exists

Use the `HasValue` property of `Resource` to perform a HEAD request and check if the resource for "Pikachu" exists. 
`HasValue` returns a `Promise<bool>` that resolves to true if the resource exists or false if it does not.

!!! info "New to promises?"

    If you haven't worked with promises before, we have written an explanation on what promises are in contrast to 
    Coroutines, [which you can find here](../explanation/promises.md).

**Example: Checking if the Resource Exists**

```csharp
pikachu.HasValue
    .Then(exists =>
    {
        if (exists)
        {
            Console.WriteLine("Pikachu exists in the Pokémon API!");
        }
        else
        {
            Console.WriteLine("Pikachu does not exist in the Pokémon API.");
        }
    })
    .Catch(error =>
    {
        Console.WriteLine($"Error checking resource existence: {error.Message}");
    });
```

In this example:

1. If the Pokémon resource exists, the promise resolves with true, and you’ll see a message confirming Pikachu’s
   existence.
2. If the resource does not exist, HasValue resolves with false, and you’ll get a message stating that Pikachu is not
   found.

## Step 3: Retrieve Pokémon Data if It Exists

If the resource exists, you can use the `Value` property of `Resource` to fetch the actual data. `Value` returns a
`Promise<T>`, where `T` is `Pokemon`, representing the model of the data fetched.

**Example: Fetching the Pokémon Data**

```csharp
pikachu.HasValue
    .Then(exists =>
    {
        if (!exists)
        {
            Console.WriteLine("The Pokémon resource does not exist; skipping data fetch.");
            return;
        }

        // Fetch the data
        pikachu.Value
            .Then(pokemon =>
            {
                Console.WriteLine($"Pokémon Name: {pokemon.Name}");
                Console.WriteLine($"Base Experience: {pokemon.BaseExperience}");
                Console.WriteLine($"Height: {pokemon.Height}");
                Console.WriteLine($"Weight: {pokemon.Weight}");
            })
            .Catch(error => Console.WriteLine($"Error fetching Pokémon data: {error.Message}"));
    })
    .Catch(error => Console.WriteLine($"Error checking resource existence: {error.Message}"));
```

In this example:

1. After confirming that the Pokémon resource exists with `HasValue`, we call `Value` to retrieve the data.
2. The promise returned by `Value` resolves with the `Pokemon` data, which we then print to the console.

**Example 2: Checking whether it exists as part of the error handling**

You can also write this in a shorter form if you do not check its existence, but instead make use of the `NotFoundError`
to check whether it could not be found while fetching it.

```csharp
pikachu.Value
   .Then(pokemon =>
   {
       Console.WriteLine($"Pokémon Name: {pokemon.Name}");
       Console.WriteLine($"Base Experience: {pokemon.BaseExperience}");
       Console.WriteLine($"Height: {pokemon.Height}");
       Console.WriteLine($"Weight: {pokemon.Weight}");
   })
   .Catch(error => {
       if (error is NotFoundError) {
           Console.WriteLine("The Pokémon resource does not exist.")
           return;
       }
       Console.WriteLine($"Error fetching Pokémon data: {error.Message}")
   });
```

## Step 4: Define the Pokémon Data Model

The `Resource` class expects a type parameter, so you’ll need a `Pokemon` class that matches the JSON structure returned
by the Pokémon API. Here’s a simple model class for Pokemon with properties for some of the primary fields.

```csharp
public class Pokemon
{
    public string Name { get; set; }
    public int BaseExperience { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
}
```

You may want to add additional properties if you need more data fields from the API.

## Summary

- **Step 1**: Create a `Resource<Pokemon>` instance for a specific Pokémon.
- **Step 2**: Use `HasValue` to check if the resource exists.
- **Step 3**: If it exists, use `Value` to fetch and display the data.

With this approach, you can manage RESTful resources with `Resource`, making it easy to check for existence and
retrieve data with asynchronous handling in a Unity-like syntax. This example demonstrates using the Pokémon API, but
you can adapt the pattern to work with any RESTful API.