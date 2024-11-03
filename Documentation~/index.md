# About Uxios

Uxios is a Unity C# library inspired by [Axios](https://axios-http.com/), the popular JavaScript HTTP client. Designed
for use with Unity, Uxios provides an intuitive and consistent API for making HTTP requests, mirroring the ease of use
and flexibility of Axios within the Unity environment. Built to be a part of the JuniSON Engine, Uxios is particularly
optimized for WebGL applications but remains compatible with Android, iOS, and Desktop builds, making it a versatile
solution for Unity projects across platforms.

## Why Uxios?

Unity's built-in UnityWebRequest API, while powerful, can be verbose and cumbersome, especially for those familiar with
modern JavaScript libraries like Axios. Uxios simplifies HTTP requests with features like promises, request
interceptors, and cleaner syntax, enabling Unity developers to build data-driven applications more easily.

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