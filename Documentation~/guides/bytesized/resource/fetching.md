# Fetching resources

## Overview

This guide demonstrates how to use the `Resource` class to fetch data from an API with built-in caching and simplified
access to RESTful resources. With just a few lines of code, you can retrieve data from a single endpoint while Uxios
handles request setup and response management for you.

---

## Steps

```csharp
// 1. Describe a `Resource` .. *At* a URL (as string) 
var resource = Resource<Post>.At("https://jsonplaceholder.typicode.com/posts/1");

// 2. Start fetching the Value using `Promise<Post>`
Promise<Post> promise = resource.Value;

// 3. Declare what needs to happen on success - a lambda is used here, but a method works too 
promise.Then(post => Debug.Log(post.title));

// 4. Declare what needs to happen on success - a lambda is used here, but a method works too 
promise.Catch(exception => Debug.LogError(exception.Message));
```

---

## Highlights

- The `At` method on a Resource helps you to describe "At what URL" you want to interact with the Resource
- Using the `Then` method you can add a callback, or delegate, that will be invoked once the data has downloaded from 
  the given URL.
- Using the `Catch` method you can add a callback, or delegate, that will be invoked if an error occurs. The exception
  in the is of type [`KindMen.Uxios.Error`](../../../reference/error.md) or one of its subtypes.

---

## Quick Tips

- If you want a more "classic" way to get a Resource instead of `Resource<TResponseType>.At(url)` you can also use 
  `new Resource<TResponseType>(url)`
- Using the `At` method you can chain actions together to configure the behaviour of a resource, such as the `With` 
  and `As` methods 
- Instead of `string`, the `At` method also supports `System.Uri` object - or more precisely: under the hood it is all
  converted to `System.Uri`.
- The `Exception` in the `Catch` part of the promise is an instance of 
  [`KindMen.Uxios.Error`](../../../reference/error.md), you can get the response data through it.

