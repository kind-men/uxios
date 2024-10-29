![Uxios Logo](./uxios.png)

Uxios is a Unity C# library inspired by [Axios](https://axios-http.com/), the popular JavaScript HTTP client. Designed 
for use with Unity, Uxios provides an intuitive and consistent API for making HTTP requests, mirroring the ease of use 
and flexibility of Axios within the Unity environment. Built to be a part of the JuniSON Engine, Uxios is particularly 
optimized for WebGL applications but remains compatible with Android, iOS, and Desktop builds, making it a versatile 
solution for Unity projects across platforms.

## Why Uxios?

Unity's built-in UnityWebRequest API, while powerful, can be verbose and cumbersome, especially for those familiar with
modern JavaScript libraries like Axios. Uxios simplifies HTTP requests with features like promises, request
interceptors, and cleaner syntax, enabling Unity developers to build data-driven applications more easily.

## Performing GET Requests

In Uxios, GET requests are designed to be simple yet flexible, assuming by default that you’re interacting with
JSON-based endpoints. Uxios automatically deserializes JSON responses into C# objects using 
[Json.NET](https://www.newtonsoft.com/json), making it easy to work with structured data in Unity.

### Basic GET Request Example (JSON Response)

By default, Uxios handles JSON responses without extra configuration. Here’s an example of fetching data from a JSON
endpoint:

```csharp
using KindMen.Uxios;
using UnityEngine;
using System;

public class ExampleGet : MonoBehaviour
{
    private void Start()
    {
        var uxios = new Uxios();
        var url = new Uri("https://jsonplaceholder.typicode.com/todos/1");

        uxios.Get(url)
            .Then(response =>
            {
                Debug.Log("Status: " + responseStatus);
                Debug.Log("Content: " + response.Data);
            })
            .Catch(error =>
            {
                Debug.LogError("Request failed: " + error.Message);
            });
    }
}
```

In this example, Uxios automatically deserializes the JSON data from the endpoint into an object accessible
via `response.Data`. When no type is specified, the JSON data is converted into a 
[JObject](https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jobject.htm) object for easy access.

## Retrieving Different Data Types

Uxios’s flexibility allows it to handle various data types directly from GET requests. By using generics, you can
specify the type of response you expect, allowing for customized processing and usage in Unity. Each request returns a
Response object, where the Data property holds the retrieved data.

### Supported Generic Types

1. **Plain text**: Use `string` as the generic type when you want to receive the data as plain text. This is ideal for
   endpoints returning text-only data, such as raw HTML or plain text files.

   ```csharp
   uxios.Get<string>(new Uri("https://example.com/text"))
       .Then(response => Debug.Log("Received text: " + response.Data))
       .Catch(error => Debug.LogError("Error: " + error.Message));
   ```

2. **Byte arrays**: Specify `byte[]` if you need binary data. This is useful for files or other binary formats, allowing
   you to handle the data at a low level.

   ```csharp
   uxios.Get<byte[]>(new Uri("https://example.com/file"))
       .Then(response => Debug.Log("Received file data of length: " + ((byte[])response.Data).Length))
       .Catch(error => Debug.LogError("Error: " + error.Message));
   ```

3. **Textures**: Uxios also supports fetching images directly as Texture2D, making it easy to retrieve images and apply 
   them in Unity.

   ```csharp
   uxios.Get<Texture2D>(new Uri("https://example.com/image.png"))
       .Then(response =>
       {
           Debug.Log("Image downloaded successfully");
           Texture2D texture = response.Data as Texture2D;
           // Use response.Data as a Texture2D
       })
       .Catch(error => Debug.LogError("Failed to fetch image: " + error.Message));
   ```

4. **Any Serializable Object Type**: For JSON-based APIs, Uxios can deserialize JSON into any custom object type that 
   is JSON-serializable. This is particularly powerful, as it leverages [Json.NET](https://www.newtonsoft.com/json) to 
   convert complex JSON responses into strongly-typed C# objects, simplifying data handling.

   ```csharp
   uxios.Get<MyCustomType>(new Uri("https://api.example.com/data"))
       .Then(response => Debug.Log("Received data: " + ((MyCustomType)response.Data).MyProperty))
       .Catch(error => Debug.LogError("Error: " + error.Message));
   ```

With Uxios’s generic type support, handling different data types is straightforward, whether working with text, binary,
images, or custom objects. This flexibility allows you to seamlessly integrate various types of data into your Unity
project.

## Why Port Axios Instead of JavaScript’s fetch API?

While fetch is a powerful API for making network requests in JavaScript, Axios' model offers several advantages that 
make it a wonderful fit for Unity development in C#. 

Key reasons for choosing Axios as the foundation for Uxios include:

1. **JSON Serialization and Deserialization**: Axios has a built-in concept of serializing data to and from JSON, which 
   aligns naturally with how Unity developers often handle data. This minimizes boilerplate code and ensures a 
   streamlined experience for handling JSON-based APIs, a common requirement in game and application development.

2. **Interceptors**: Axios’s native support for request and response interceptors opens up a world of possibilities. Unity
   developers can use interceptors to log network calls, handle authentication, or add error handling with ease. In a
   debugging context, for example, interceptors allow for seamless logging of all networking calls, giving developers
   insight into their application's network behavior without invasive code changes.

3. **Leverage C#’s Strong Typing**: The Axios-inspired approach in Uxios plays well with C#’s powerful type system. 
   Developers can define and enforce types more strictly than in JavaScript, reducing runtime errors and making code 
   easier to maintain and refactor.

This combination of JSON handling, flexible interceptors, and strong typing in C# positions Uxios as a highly
productive, efficient library for Unity developers building network-reliant applications.